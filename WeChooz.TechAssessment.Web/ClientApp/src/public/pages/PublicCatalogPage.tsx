import { useState, useMemo } from 'react';
import { useGetAllSessions } from '../../hooks/sessions/useGetAllSessions';
import FilterSidebar, { SessionFilters } from '../components/FilterSidebar';
import SessionCard from '../components/SessionCard';
import SessionDetailModal from '../components/SessionDetailModal';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';
// If you have login/logout functions, import them here, e.g.:
// import { useAuth } from '../../hooks/auth/useAuth';

// ... (Interface Session inchangée)

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
            const matchPopulation = !filters.population || session.course.targetAudience === filters.population;
            const matchMode = !filters.mode || session.deliveryMode === filters.mode;

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
            <div className="flex justify-center items-center min-h-screen">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Barre de Navigation / Header */}
            <nav className="sticky top-0 z-10 bg-white/80 backdrop-blur-md border-b border-gray-200 px-4 py-3 mb-6">
                <div className="max-w-7xl mx-auto flex justify-between items-center">
                    <h1 className="text-xl font-bold text-gray-800">Catalogue de Formations</h1>

                    <div>
                        {isLoggedIn ? (
                            <button
                                onClick={() => logout?.()}
                                className="px-5 py-2 text-sm font-medium text-red-600 hover:bg-red-50 border border-red-200 rounded-lg transition-colors"
                            >
                                Se déconnecter
                            </button>
                        ) : (
                            <button
                                onClick={() => login?.()}
                                className="px-5 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg shadow-sm transition-all"
                            >
                                Connexion
                            </button>
                        )}
                    </div>
                </div>
            </nav>

            <div className="max-w-7xl mx-auto flex flex-col md:flex-row gap-8 p-4 md:p-8">
                {/* Sidebar de filtres */}
                <FilterSidebar filters={filters} setFilters={setFilters} />

                {/* Liste des résultats */}
                <div className="flex-1">
                    <div className="mb-4 text-sm text-gray-500 italic">
                        {filteredSessions.length} session(s) trouvée(s) {isLoggedIn && "• Bienvenue, " + user?.username}!
                    </div>

                    <div className="grid grid-cols-1 gap-6">
                        {filteredSessions.length > 0 ? (
                            filteredSessions.map((session) => (
                                <SessionCard
                                    key={session.id}
                                    onViewDetails={() => setSelectedSessionId(session.id)}
                                    session={session}
                                />
                            ))
                        ) : (
                            <div className="bg-white p-12 text-center rounded-xl border border-dashed border-gray-300">
                                <p className="text-gray-500">Aucune session ne correspond à vos critères.</p>
                            </div>
                        )}
                    </div>
                </div>
            </div>

            {/* Modal de détails */}
            {selectedSessionId && (
                <SessionDetailModal
                    sessionId={selectedSessionId}
                    onClose={() => setSelectedSessionId(null)}
                />
            )}
        </div>
    );
};

export default PublicCatalogPage;