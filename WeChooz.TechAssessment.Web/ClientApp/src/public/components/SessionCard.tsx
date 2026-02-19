import { TargetAudience, targetAudienceLabels } from '../../services/courses/TargetAudience';
import { DeliveryMode, deliveryModeLabels } from '../../services/sessions/DeliveryMode';
import { SessionDTO } from '../../services/sessions/SessionDTO';
import { Card, Text, Badge, Button, Group, Stack, Grid, Box } from '@mantine/core';

interface SessionCardProps {
    session: SessionDTO;
    onViewDetails: () => void;
}

const SessionCard = ({ session, onViewDetails }: SessionCardProps) => {
    // Formater la date proprement
    const formattedDate = new Date(session.startDate).toLocaleDateString('fr-FR', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
    });

    const isFull = session.userCount <= 0;
    const remainingPlaces = session.userCount;

    return (
        <Card shadow="sm" padding="lg" radius="md" withBorder>
            <Grid>
                <Grid.Col span={{ base: 12, sm: 1 }} style={{ display: 'flex', alignItems: 'stretch' }}>
                    <Box
                        bg={session.deliveryMode === DeliveryMode.InPerson ? 'blue' : 'green'}
                        style={{ width: '8px', borderRadius: '4px', height: '100%' }}
                    />
                </Grid.Col>

                <Grid.Col span={{ base: 12, sm: 11 }}>
                    <Stack gap="xs">
                        <Group justify="space-between" align="start">
                            <div>
                                <Badge color="indigo" variant="light" mb="xs">
                                    {targetAudienceLabels[session.course.targetAudience as TargetAudience] ?? session.course.targetAudience}
                                </Badge>
                                <Text fw={700} size="lg" c="dark">
                                    {session.courseName}
                                </Text>
                            </div>
                            <Text size="sm" fw={700} c={remainingPlaces <= 3 ? 'red' : 'dimmed'}>
                                {remainingPlaces} places restantes
                            </Text>
                        </Group>

                        <Text size="sm" c="dimmed" lineClamp={2}>
                            {session.course.shortDescription}
                        </Text>

                        <Grid mt="md" style={{ borderTop: '1px solid var(--mantine-color-gray-2)' }} pt="sm">
                            <Grid.Col span={6} md={3}>
                                <Text size="xs" tt="uppercase" c="dimmed">Date</Text>
                                <Text size="sm" fw={500}>{formattedDate}</Text>
                            </Grid.Col>
                            <Grid.Col span={6} md={3}>
                                <Text size="xs" tt="uppercase" c="dimmed">Durée</Text>
                                <Text size="sm" fw={500}>{session.course.durationInDays} jour(s)</Text>
                            </Grid.Col>
                            <Grid.Col span={6} md={3}>
                                <Text size="xs" tt="uppercase" c="dimmed">Mode</Text>
                                <Text size="sm" fw={500}>
                                    {deliveryModeLabels[session.deliveryMode as DeliveryMode] ?? session.deliveryMode}
                                </Text>
                            </Grid.Col>
                            <Grid.Col span={6} md={3}>
                                <Text size="xs" tt="uppercase" c="dimmed">Formateur</Text>
                                <Text size="sm" fw={500}>{session.course.trainerLastName}</Text>
                            </Grid.Col>
                        </Grid>

                        <Group justify="flex-end" mt="md">
                            <Button variant="outline" onClick={onViewDetails}>
                                Voir détails
                            </Button>
                        </Group>
                    </Stack>
                </Grid.Col>
            </Grid>
        </Card>
    );
};

export default SessionCard;
