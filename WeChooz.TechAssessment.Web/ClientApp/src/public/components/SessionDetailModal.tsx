import { useEffect, useState } from 'react';
import { useGetSessionById } from '../../hooks/sessions/useGetSessionById';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';
import { useEnrollSession } from '../../hooks/sessions/useEnrollSession';
import { Modal, Button, Text, Group, Stack, Loader, Alert, Badge, Grid, Divider, Box, TypographyStylesProvider } from '@mantine/core';
import { IconAlertTriangle, IconCheck } from '@tabler/icons-react';

interface Props {
    sessionId: string;
    onClose: () => void;
}

const SessionDetailModal = ({ sessionId, onClose }: Props) => {
    const { session, loading, error } = useGetSessionById(sessionId);
    const { isLoggedIn, login } = useCurrentUser();
    const { mutate: enroll } = useEnrollSession();
    const [errorMessage, setErrorMessage] = useState<string | null>(null);

    const handleRegisterClick = () => {
        if (!isLoggedIn) {
            login();
            return;
        }

        enroll(sessionId, {
            onSuccess: () => {
                onClose();
            },
            onError: async (error: any) => {
                let message = "Une erreur est survenue lors de l'inscription.";
                if (error.response) {
                    try {
                        const data = await error.response.json();
                        message = data.message || message;
                    } catch {
                        // ignore
                    }
                } else if (error.message) {
                    message = error.message;
                }
                setErrorMessage(message);
            }
        });
    };

    if (loading) {
        return (
            <Modal opened={true} onClose={onClose} withCloseButton={false} centered>
                <Stack align="center" py="xl">
                    <Loader size="lg" />
                    <Text c="dimmed">Récupération des détails...</Text>
                </Stack>
            </Modal>
        );
    }

    if (error || !session) {
        return (
            <Modal opened={true} onClose={onClose} title="Erreur" centered>
                <Stack align="center" py="md">
                    <IconAlertTriangle size={40} color="red" />
                    <Text fw={700} size="lg">Oups !</Text>
                    <Text c="dimmed">{error ? "Impossible de charger la session." : "Session introuvable."}</Text>
                    <Button fullWidth onClick={onClose} variant="light" color="gray">Fermer</Button>
                </Stack>
            </Modal>
        );
    }

    const { course, userCount } = session;
    const remainingPlaces = userCount;

    return (
        <Modal
            opened={true}
            onClose={onClose}
            title={<Text fw={900} size="xl">{course.name}</Text>}
            size="lg"
            centered
            overlayProps={{
                backgroundOpacity: 0.55,
                blur: 3,
            }}
        >
            {errorMessage && (
                <Alert icon={<IconAlertTriangle size={16} />} title="Erreur" color="red" mb="md" withCloseButton onClose={() => setErrorMessage(null)}>
                    {errorMessage}
                </Alert>
            )}

            <Stack gap="md">
                <Grid>
                    <Grid.Col span={4}>
                        <PaperInfo label="Formateur" value={`${course.trainerFirstName} ${course.trainerLastName}`} />
                    </Grid.Col>
                    <Grid.Col span={4}>
                        <PaperInfo label="Durée" value={`${course.durationInDays} jour(s)`} />
                    </Grid.Col>
                    <Grid.Col span={4}>
                        <PaperInfo
                            label="Disponibilité"
                            value={remainingPlaces > 0 ? `${remainingPlaces} places` : 'Complet'}
                            color={remainingPlaces <= 2 ? 'red' : 'green'}
                        />
                    </Grid.Col>
                </Grid>

                <Text fw={700} size="lg" mt="md">À propos de cette formation</Text>

                <TypographyStylesProvider>
                    <div dangerouslySetInnerHTML={{ __html: course.longDescription || '<em>Pas de description détaillée.</em>' }} />
                </TypographyStylesProvider>

                <Divider my="sm" />

                <Group justify="space-between" align="center">
                    <Text size="xs" c="dimmed" fs="italic">ID Session: {sessionId}</Text>
                    <Group>
                        <Button variant="default" onClick={onClose}>Annuler</Button>
                        <Button
                            onClick={handleRegisterClick}
                            disabled={remainingPlaces <= 0}
                            color="blue"
                        >
                            {remainingPlaces <= 0 ? 'Complet' : "S'inscrire"}
                        </Button>
                    </Group>
                </Group>
            </Stack>
        </Modal>
    );
};

const PaperInfo = ({ label, value, color }: { label: string, value: string, color?: string }) => (
    <Box p="xs" style={{ border: '1px solid var(--mantine-color-gray-3)', borderRadius: 'var(--mantine-radius-md)', backgroundColor: 'var(--mantine-color-gray-0)' }}>
        <Text size="xs" tt="uppercase" c="dimmed" fw={700}>{label}</Text>
        <Text size="sm" fw={600} c={color}>{value}</Text>
    </Box>
);

export default SessionDetailModal;
