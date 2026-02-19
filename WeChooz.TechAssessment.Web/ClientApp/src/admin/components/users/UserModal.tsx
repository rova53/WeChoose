import React, { useState, useEffect } from 'react';
import { useCreateUser } from '../../../hooks/users/useCreateUser';
import { useUpdateUser } from '../../../hooks/users/useUpdateUser';
import { UserDTO } from '../../../services/users/UserDTO';
import { useGetAllSessions } from '../../../hooks/sessions/useGetAllSessions';
import { SessionEnroll } from '../../../services/sessions/SessionEnrollDTO';
import { Modal, TextInput, PasswordInput, Button, Group, Stack, Checkbox, ScrollArea, Text, Alert, Grid, Paper } from '@mantine/core';
import { IconAlertTriangle } from '@tabler/icons-react';

interface UserModalProps {
    user?: UserDTO | null; // Si présent = Mode Édition
    onClose: () => void;
    onSuccess: () => void;
}

export const UserModal: React.FC<UserModalProps> = ({ user, onClose, onSuccess }) => {
    const isEditMode = !!user;
    const { mutate: createUser, loading: creating, error: createError } = useCreateUser();
    const { mutate: updateUser, loading: updating, error: updateError } = useUpdateUser();
    const { sessions = [] } = useGetAllSessions();

    const [formData, setFormData] = useState<Partial<UserDTO>>({
        lastName: '',
        firstName: '',
        email: '',
        companyName: '',
        password: '',
    });

    const [selectedSessions, setSelectedSessions] = useState<string[]>([]);

    useEffect(() => {
        if (user) {
            setFormData({
                lastName: user.lastName || '',
                firstName: user.firstName || '',
                email: user.email || '',
                companyName: user.companyName || '',
            });
            const existingSessionIds = user.enrollments?.map(e => e.sessionId).filter(Boolean) as string[] || [];
            setSelectedSessions(existingSessionIds);
        }
    }, [user]);

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleCheckboxChange = (sessionId: string) => {
        setSelectedSessions(prev =>
            prev.includes(sessionId) ? prev.filter(id => id !== sessionId) : [...prev, sessionId]
        );
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const enrollments: Partial<SessionEnroll>[] = selectedSessions.map(id => ({
            sessionId: id,
            enrollmentDate: new Date().toISOString(),
        }));

        const payload = { ...formData, id: user?.id, enrollments: enrollments as SessionEnroll[] };
        let result;
        if (isEditMode && user?.id) {
            result = await updateUser({ id: user.id, user: payload as UserDTO });
        } else {
            result = await createUser(payload as UserDTO);
        }

        if (result) onSuccess();
    };

    const loading = creating || updating;
    const error = createError || updateError;

    return (
        <Modal
            opened={true}
            onClose={onClose}
            title={<Text fw={700} size="lg">{isEditMode ? `Modifier : ${user?.firstName} ${user?.lastName}` : 'Créer un utilisateur'}</Text>}
            size="lg"
            centered
            overlayProps={{
                backgroundOpacity: 0.55,
                blur: 3,
            }}
        >
            <form onSubmit={handleSubmit}>
                <Stack gap="md">
                    <Grid>
                        <Grid.Col span={6}>
                            <TextInput
                                label="Nom"
                                name="lastName"
                                value={formData.lastName}
                                onChange={handleChange}
                                required
                                withAsterisk
                                placeholder="Doe"
                            />
                        </Grid.Col>
                        <Grid.Col span={6}>
                            <TextInput
                                label="Prénom"
                                name="firstName"
                                value={formData.firstName}
                                onChange={handleChange}
                                required
                                withAsterisk
                                placeholder="John"
                            />
                        </Grid.Col>
                    </Grid>

                    <TextInput
                        label="Email"
                        name="email"
                        type="email"
                        value={formData.email}
                        onChange={handleChange}
                        required
                        withAsterisk
                        placeholder="john.doe@example.com"
                    />

                    {!isEditMode && (
                        <PasswordInput
                            label="Mot de passe"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            required
                            withAsterisk
                            placeholder="********"
                        />
                    )}

                    <TextInput
                        label="Entreprise"
                        name="companyName"
                        value={formData.companyName}
                        onChange={handleChange}
                        placeholder="Acme Corp"
                    />

                    <Stack gap="xs">
                        <Text size="sm" fw={500}>Inscrire à des sessions</Text>
                        <Paper withBorder p="xs" h={150}>
                            <ScrollArea h="100%" type="auto" offsetScrollbars>
                                <Stack gap="xs">
                                    {sessions.length > 0 ? sessions.map(session => (
                                        <Checkbox
                                            key={session.id}
                                            label={`${session.courseName} - ${new Date(session.startDate).toLocaleDateString()}`}
                                            checked={selectedSessions.includes(session.id)}
                                            onChange={() => handleCheckboxChange(session.id)}
                                        />
                                    )) : (
                                        <Text size="sm" c="dimmed" ta="center" mt="xl">Aucune session disponible</Text>
                                    )}
                                </Stack>
                            </ScrollArea>
                        </Paper>
                    </Stack>

                    {error && (
                        <Alert icon={<IconAlertTriangle size={16} />} title="Erreur" color="red" variant="light">
                            {error.message}
                        </Alert>
                    )}

                    <Group justify="flex-end" mt="md">
                        <Button variant="default" onClick={onClose}>Annuler</Button>
                        <Button type="submit" loading={loading}>
                            {isEditMode ? 'Enregistrer' : 'Créer'}
                        </Button>
                    </Group>
                </Stack>
            </form>
        </Modal>
    );
};
