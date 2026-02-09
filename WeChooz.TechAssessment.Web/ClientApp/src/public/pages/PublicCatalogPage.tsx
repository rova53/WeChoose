import { useState, useMemo } from 'react';
import { useGetAllSessions } from '../../hooks/sessions/useGetAllSessions';
import FilterSidebar, { SessionFilters } from '../components/FilterSidebar';
import SessionCard from '../components/SessionCard';
import SessionDetailModal from '../components/SessionDetailModal';
import { CourseDTO } from '../../services/courses/CourseDTO';


export interface Session {
    id: string;
    courseId: string;
    courseName: string;
    startDate: string;
    deliveryMode: string;
    userCount: number;
    maxParticipants: number;
    course: CourseDTO;
    trainerName?: string;
}

const PublicCatalogPage = () => {
    const { sessions, loading } = useGetAllSessions();
    const [selectedSessionId, setSelectedSessionId] = useState<string | null>(null);

    const [filters, setFilters] = useState<SessionFilters>({
        population: '',
        mode: '',
        startDate: '',
        dateType: '',
        endDate: ''
    });

    // Logique de filtrage optimisée
    const filteredSessions = useMemo(() => {
        if (!sessions) return [];

        return sessions.filter(session => {
            // 1. Filtre Population
            const matchPopulation = !filters.population || session.course.targetAudience === filters.population;

            // 2. Filtre Mode
            const matchMode = !filters.mode || session.deliveryMode === filters.mode;

            // 3. Filtre Dates
            let matchDate = true;
            const sessionDate = new Date(session.startDate).getTime();
            const filterStart = filters.startDate ? new Date(filters.startDate).getTime() : null;
            const filterEnd = filters.endDate ? new Date(filters.endDate).getTime() : null;

            if (filters.dateType && filterStart) {
                if (filters.dateType === 'before') {
                    matchDate = sessionDate <= filterStart;
                } else if (filters.dateType === 'after') {
                    matchDate = sessionDate >= filterStart;
                } else if (filters.dateType === 'between' && filterEnd) {
                    matchDate = sessionDate >= filterStart && sessionDate <= filterEnd;
                }
            }

            return matchPopulation && matchMode && matchDate;
        });
    }, [sessions, filters]);

    if (loading) {
        return (
            <div className="flex justify-center items-center min-h-screen">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
            </div>
        );
    }

    return (
        <div className="max-w-7xl mx-auto flex flex-col md:flex-row gap-8 p-4 md:p-8 bg-gray-50 min-h-screen">
            {/* Sidebar de filtres */}
            <FilterSidebar filters={filters} setFilters={setFilters} />

            {/* Liste des résultats */}
            <div className="flex-1">
                <div className="mb-4 text-sm text-gray-500 italic">
                    {filteredSessions.length} session(s) trouvée(s)
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