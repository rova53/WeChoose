import { useState, useMemo } from 'react';
import { useGetAllSessions } from '../../hooks/sessions/useGetAllSessions';
import FilterSidebar, { SessionFilters } from '../components/FilterSidebar';
import SessionCard from '../components/SessionCard';
import SessionDetailModal from '../components/SessionDetailModal';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';
import { TargetAudience } from '../../services/courses/TargetAudience';
import { DeliveryMode } from '../../services/sessions/DeliveryMode';
import {
    AppShell,
    Container,
    Title,
    Text,
    Button,
    Group,
    Loader,
    Center,
    Grid,
    Paper,
    Stack,
    Box
} from '@mantine/core';

const PublicCatalogPage = () => {
    const { sessions, loading } = useGetAllSessions();
    const [selectedSessionId, setSelectedSessionId] = useState<string | null>(null);

    const { user, isLoggedIn, loading: authLoading, logout, login } = useCurrentUser?.() ?? {};

    const [filters, setFilters] = useState<SessionFilters>({
        population: '',
        mode: '',
        startDate: '',
        dateType: '',
        endDate: ''
    });

    const filteredSessions = useMemo(() => {
        if (!sessions) return [];
        return sessions.filter(session => {
            const matchPopulation = !filters.population || Number(filters.population) as TargetAudience === session.course.targetAudience;
            const matchMode = !filters.mode || session.deliveryMode === Number(filters.mode) as DeliveryMode;

            let matchDate = true;
            const sessionDate = new Date(session.startDate).getTime();
            const filterStart = filters.startDate ? new Date(filters.startDate).getTime() : null;
            const filterEnd = filters.endDate ? new Date(filters.endDate).getTime() : null;

            if (filters.dateType && filterStart) {
                if (filters.dateType === 'before') matchDate = sessionDate <= filterStart;
                else if (filters.dateType === 'after') matchDate = sessionDate >= filterStart;
                else if (filters.dateType === 'between' && filterEnd) {
                    matchDate = sessionDate >= filterStart && sessionDate <= filterEnd;
                }
            }
            return matchPopulation && matchMode && matchDate;
        });
    }, [sessions, filters]);

    if (loading || authLoading) {
        return (
            <Center h="100vh">
                <Loader size="xl" />
            </Center>
        );
    }

    return (
        <Box bg="gray.0" mih="100vh">
            <AppShell header={{ height: 60 }} padding="md">
                <AppShell.Header>
                    <Container size="xl" h="100%">
                        <Group justify="space-between" h="100%">
                            <Title order={3}>Catalogue de Formations</Title>
                            <Group>
                                {isLoggedIn ? (
                                    <Button
                                        variant="outline"
                                        color="red"
                                        onClick={() => logout?.()}
                                    >
                                        Se déconnecter
                                    </Button>
                                ) : (
                                    <Button
                                        onClick={() => login?.()}
                                    >
                                        Connexion
                                    </Button>
                                )}
                            </Group>
                        </Group>
                    </Container>
                </AppShell.Header>

                <AppShell.Main>
                    <Container size="xl" py="xl">
                        <Grid gutter="xl">
                            <Grid.Col span={{ base: 12, md: 3 }}>
                                <FilterSidebar filters={filters} setFilters={setFilters} />
                            </Grid.Col>

                            <Grid.Col span={{ base: 12, md: 9 }}>
                                <Stack gap="md">
                                    <Text size="sm" c="dimmed" fs="italic">
                                        {filteredSessions.length} session(s) trouvée(s) {isLoggedIn && `• Bienvenue, ${user?.username}!`}
                                    </Text>

                                    {filteredSessions.length > 0 ? (
                                        <Stack gap="md">
                                            {filteredSessions.map((session) => (
                                                <SessionCard
                                                    key={session.id}
                                                    onViewDetails={() => setSelectedSessionId(session.id)}
                                                    session={session}
                                                />
                                            ))}
                                        </Stack>
                                    ) : (
                                        <Paper p="xl" withBorder style={{ borderStyle: 'dashed', textAlign: 'center' }}>
                                            <Text c="dimmed">Aucune session ne correspond à vos critères.</Text>
                                        </Paper>
                                    )}
                                </Stack>
                            </Grid.Col>
                        </Grid>
                    </Container>
                </AppShell.Main>
            </AppShell>

            {selectedSessionId && (
                <SessionDetailModal
                    sessionId={selectedSessionId}
                    onClose={() => setSelectedSessionId(null)}
                />
            )}
        </Box>
    );
};

export default PublicCatalogPage;
