import React, { useState, useEffect } from 'react';
import { useCreateSession } from '../../../hooks/sessions/useCreateSession';
import { useUpdateSession } from '../../../hooks/sessions/useUpdateSession';
import { useGetAllCourses } from '../../../hooks/courses/useGetAllCourses';
import { SessionDTO } from '../../../services/sessions/SessionDTO';
import { DeliveryMode, deliveryModeLabels } from '../../../services/sessions/DeliveryMode';
import { Modal, Select, TextInput, NumberInput, Button, Group, Stack, Text, Alert, Grid } from '@mantine/core';
import { IconAlertTriangle } from '@tabler/icons-react';

interface SessionModalProps {
    session?: SessionDTO | null;
    onClose: () => void;
    onSuccess: () => void;
}

export const SessionModal: React.FC<SessionModalProps> = ({
    session,
    onClose,
    onSuccess
}) => {
    const isEditMode = !!session;
    const { mutate: createSession, loading: creating, error: createError } = useCreateSession();
    const { mutate: updateSession, loading: updating, error: updateError } = useUpdateSession();
    const { courses, loading: loadingCourses } = useGetAllCourses();

    const [formData, setFormData] = useState({
        courseId: '',
        courseName: '',
        startDate: '',
        deliveryMode: '' as string | number,
        UserCount: 0,
    });

    useEffect(() => {
        if (session) {
            setFormData({
                courseId: session.courseId || '',
                courseName: session.courseName || '',
                startDate: session.startDate ? new Date(session.startDate).toISOString().split('T')[0] : '',
                deliveryMode: session.deliveryMode ?? '',
                UserCount: session.UserCount || 0,
            });
        }
    }, [session]);

    const handleChange = (name: string, value: any) => {
        if (name === 'courseId') {
            const selectedCourse = courses.find(c => c.id === value);
            setFormData(prev => ({
                ...prev,
                courseId: value,
                courseName: selectedCourse ? selectedCourse.name : ''
            }));
        } else if (name === 'deliveryMode') {
            setFormData(prev => ({
                ...prev,
                deliveryMode: Number(value)
            }));
        } else {
            setFormData(prev => ({
                ...prev,
                [name]: name === 'UserCount' ? Number(value) : value
            }));
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const payload = {
            id: session?.id,
            ...formData,
            deliveryMode: Number(formData.deliveryMode) as DeliveryMode
        };

        let result;
        if (isEditMode && session?.id) {
            result = await updateSession({ id: session.id, session: payload as SessionDTO });
        } else {
            result = await createSession(payload as Omit<SessionDTO, 'id'>);
        }

        if (result) {
            onSuccess();
        }
    };

    const loading = creating || updating;
    const error = createError || updateError;

    return (
        <Modal
            opened={true}
            onClose={onClose}
            title={<Text fw={700} size="lg">{isEditMode ? 'Modifier la session' : 'Planifier une session'}</Text>}
            size="lg"
            centered
            overlayProps={{
                backgroundOpacity: 0.55,
                blur: 3,
            }}
        >
            <form onSubmit={handleSubmit}>
                <Stack gap="md">
                    {!isEditMode ? (
                        <Select
                            label="Sélectionner le cours"
                            data={courses.map(course => ({ value: course.id, label: course.name }))}
                            value={formData.courseId}
                            onChange={(value) => handleChange('courseId', value)}
                            disabled={loadingCourses}
                            required
                            withAsterisk
                            placeholder={loadingCourses ? 'Chargement...' : 'Rechercher un cours...'}
                            searchable
                        />
                    ) : (
                        <TextInput
                            label="Cours"
                            value={formData.courseName}
                            disabled
                        />
                    )}

                    <Grid>
                        <Grid.Col span={6}>
                            <TextInput
                                type="date"
                                label="Date de début"
                                value={formData.startDate}
                                onChange={(e) => handleChange('startDate', e.target.value)}
                                required
                                withAsterisk
                            />
                        </Grid.Col>
                        <Grid.Col span={6}>
                            <Select
                                label="Mode de diffusion"
                                data={Object.keys(deliveryModeLabels).map((key) => ({
                                    value: key,
                                    label: deliveryModeLabels[Number(key) as DeliveryMode]
                                }))}
                                value={String(formData.deliveryMode)}
                                onChange={(value) => handleChange('deliveryMode', value)}
                                required
                                withAsterisk
                                placeholder="Choisir..."
                            />
                        </Grid.Col>
                    </Grid>

                    <NumberInput
                        label="Nombre de participants maximum"
                        description="Si différent de la capacité du cours"
                        value={formData.UserCount}
                        onChange={(value) => handleChange('UserCount', value)}
                        min={0}
                    />

                    {error && (
                        <Alert icon={<IconAlertTriangle size={16} />} title="Erreur" color="red" variant="light">
                            {error.message}
                        </Alert>
                    )}

                    <Group justify="flex-end" mt="md">
                        <Button variant="default" onClick={onClose}>Annuler</Button>
                        <Button
                            type="submit"
                            loading={loading}
                            disabled={!isEditMode && !formData.courseId}
                        >
                            {isEditMode ? 'Enregistrer' : 'Planifier'}
                        </Button>
                    </Group>
                </Stack>
            </form>
        </Modal>
    );
};
