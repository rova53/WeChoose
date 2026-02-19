import React, { useState, useEffect } from 'react';
import { useCreateCourse } from '../../../hooks/courses/useCreateCourse';
import { useUpdateCourse } from '../../../hooks/courses/useUpdateCourse';
import { CourseDTO } from '../../../services/courses/CourseDTO';
import { targetAudienceLabels } from "../../../services/courses/TargetAudience.ts";
import { Modal, TextInput, Textarea, Select, NumberInput, Button, Group, Stack, Text, Alert, Grid } from '@mantine/core';
import { IconAlertTriangle } from '@tabler/icons-react';

interface CourseModalProps {
    course?: CourseDTO | null;
    onClose: () => void;
    onSuccess: () => void;
}

export const CourseModal: React.FC<CourseModalProps> = ({
    course,
    onClose,
    onSuccess
}) => {
    const isEditMode = !!course;
    const { mutate: createCourse, loading: creating, error: createError } = useCreateCourse();
    const { mutate: updateCourse, loading: updating, error: updateError } = useUpdateCourse();

    const loading = creating || updating;
    const error = createError || updateError;

    const [formData, setFormData] = useState({
        name: '',
        shortDescription: '',
        longDescription: '',
        durationInDays: '',
        targetAudience: '',
        maxCapacity: '',
        trainerFirstName: '',
        trainerLastName: '',
    });

    useEffect(() => {
        if (course) {
            setFormData({
                name: course.name || '',
                shortDescription: course.shortDescription || '',
                longDescription: course.longDescription || '',
                durationInDays: course.durationInDays?.toString() || '',
                targetAudience: course.targetAudience?.toString() || '',
                maxCapacity: course.maxCapacity?.toString() || '',
                trainerFirstName: course.trainerFirstName || '',
                trainerLastName: course.trainerLastName || '',
            });
        }
    }, [course]);

    const handleChange = (name: string, value: any) => {
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const payload = {
            id: course?.id,
            name: formData.name,
            shortDescription: formData.shortDescription,
            longDescription: formData.longDescription,
            durationInDays: Number(formData.durationInDays),
            targetAudience: Number(formData.targetAudience),
            maxCapacity: Number(formData.maxCapacity),
            trainerFirstName: formData.trainerFirstName,
            trainerLastName: formData.trainerLastName,
        };

        let result;
        if (isEditMode && course) {
            result = await updateCourse({
                id: course.id,
                course: payload
            });
        } else {
            result = await createCourse(payload);
        }

        if (result) {
            onSuccess();
        }
    };

    return (
        <Modal
            opened={true}
            onClose={onClose}
            title={<Text fw={700} size="lg">{isEditMode ? 'Modifier le cours' : 'Créer un nouveau cours'}</Text>}
            size="lg"
            centered
            overlayProps={{
                backgroundOpacity: 0.55,
                blur: 3,
            }}
        >
            <form onSubmit={handleSubmit}>
                <Stack gap="md">
                    <TextInput
                        label="Titre du cours"
                        value={formData.name}
                        onChange={(e) => handleChange('name', e.target.value)}
                        required
                        withAsterisk
                        placeholder="Ex: Introduction à React"
                    />

                    <Grid>
                        <Grid.Col span={6}>
                            <TextInput
                                label="Description courte"
                                value={formData.shortDescription}
                                onChange={(e) => handleChange('shortDescription', e.target.value)}
                                placeholder="Résumé en une phrase"
                            />
                        </Grid.Col>
                        <Grid.Col span={6}>
                            <Select
                                label="Population cible"
                                data={Object.entries(targetAudienceLabels).map(([value, label]) => ({
                                    value,
                                    label
                                }))}
                                value={formData.targetAudience}
                                onChange={(value) => handleChange('targetAudience', value)}
                                required
                                withAsterisk
                                placeholder="Choisir..."
                            />
                        </Grid.Col>
                    </Grid>

                    <Textarea
                        label="Description longue"
                        value={formData.longDescription}
                        onChange={(e) => handleChange('longDescription', e.target.value)}
                        minRows={3}
                        placeholder="Détails du contenu, pré-requis, objectifs..."
                    />

                    <Grid>
                        <Grid.Col span={6}>
                            <NumberInput
                                label="Durée (jours)"
                                value={formData.durationInDays ? Number(formData.durationInDays) : ''}
                                onChange={(value) => handleChange('durationInDays', value)}
                                min={1}
                                placeholder="1"
                            />
                        </Grid.Col>
                        <Grid.Col span={6}>
                            <NumberInput
                                label="Capacité max."
                                value={formData.maxCapacity ? Number(formData.maxCapacity) : ''}
                                onChange={(value) => handleChange('maxCapacity', value)}
                                min={1}
                                placeholder="10"
                            />
                        </Grid.Col>
                    </Grid>

                    <Grid>
                        <Grid.Col span={6}>
                            <TextInput
                                label="Prénom du formateur"
                                value={formData.trainerFirstName}
                                onChange={(e) => handleChange('trainerFirstName', e.target.value)}
                                placeholder="Jean"
                            />
                        </Grid.Col>
                        <Grid.Col span={6}>
                            <TextInput
                                label="Nom du formateur"
                                value={formData.trainerLastName}
                                onChange={(e) => handleChange('trainerLastName', e.target.value)}
                                placeholder="Dupont"
                            />
                        </Grid.Col>
                    </Grid>

                    {error && (
                        <Alert icon={<IconAlertTriangle size={16} />} title="Erreur" color="red" variant="light">
                            {error.message}
                        </Alert>
                    )}

                    <Group justify="flex-end" mt="md">
                        <Button variant="default" onClick={onClose}>Annuler</Button>
                        <Button type="submit" loading={loading}>
                            {isEditMode ? 'Enregistrer' : 'Créer le cours'}
                        </Button>
                    </Group>
                </Stack>
            </form>
        </Modal>
    );
};
