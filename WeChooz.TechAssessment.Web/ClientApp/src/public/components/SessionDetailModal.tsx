import { useEffect, useState } from 'react';
import { useGetSessionById } from '../../hooks/sessions/useGetSessionById';
import { useCurrentUser } from '../../hooks/auth/useCurrentUser';
import { useEnrollSession } from '../../hooks/sessions/useEnrollSession';

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
        console.log('sessionId dans handleRegisterClick:', sessionId);
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

    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (e.key === 'Escape') onClose();
        };
        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [onClose]);

    const ModalOverlay = ({ children }: { children: React.ReactNode }) => (
        <div
            className="fixed inset-0 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center p-4 z-50"
            onClick={(e) => e.target === e.currentTarget && onClose()}
        >
            {children}
        </div>
    );

    if (loading) return (
        <ModalOverlay>
            <div className="bg-white rounded-2xl p-10 flex flex-col items-center shadow-xl">
                <div className="animate-spin rounded-full h-10 w-10 border-4 border-blue-100 border-t-blue-600 mb-4" />
                <p className="text-slate-600 font-medium">Récupération des détails...</p>
            </div>
        </ModalOverlay>
    );

    if (error || !session) return (
        <ModalOverlay>
            <div className="bg-white rounded-2xl p-8 flex flex-col items-center max-w-sm text-center shadow-xl">
                <div className="text-red-500 text-4xl mb-4">⚠️</div>
                <h3 className="text-lg font-bold text-slate-900 mb-2">Oups !</h3>
                <p className="text-slate-500 mb-6">{error ? "Impossible de charger la session." : "Session introuvable."}</p>
                <button onClick={onClose} className="w-full bg-slate-100 hover:bg-slate-200 text-slate-700 font-semibold py-2 rounded-xl transition-colors">
                    Fermer
                </button>
            </div>
        </ModalOverlay>
    );

    const { course, deliveryMode, userCount } = session;
    const remainingPlaces = userCount;

    // Modal d'erreur
    if (errorMessage) {
        return (
            <ModalOverlay>
                <div className="bg-white rounded-2xl p-8 flex flex-col items-center max-w-sm text-center shadow-xl">
                    <div className="text-red-500 text-4xl mb-4">⚠️</div>
                    <h3 className="text-lg font-bold text-slate-900 mb-2">Erreur</h3>
                    <p className="text-slate-500 mb-6">{errorMessage}</p>
                    <button
                        onClick={() => setErrorMessage(null)}
                        className="w-full bg-slate-100 hover:bg-slate-200 text-slate-700 font-semibold py-2 rounded-xl transition-colors"
                    >
                        Fermer
                    </button>
                </div>
            </ModalOverlay>
        );
    }

    return (
        <ModalOverlay>
            <div
                className="bg-white rounded-2xl max-w-3xl w-full max-h-[90vh] overflow-hidden shadow-2xl flex flex-col relative animate-in zoom-in-95 duration-200"
                role="dialog"
                aria-modal="true"
            >
                <div className="p-6 border-b border-slate-100 flex justify-between items-start bg-slate-50/50">
                    <div>
                        <h2 className="text-2xl font-black text-slate-900 leading-tight">
                            {course.name}
                        </h2>
                    </div>
                    <button
                        onClick={onClose}
                        className="text-slate-400 hover:text-slate-600 p-2 hover:bg-slate-200 rounded-full transition-colors text-xl"
                        aria-label="Fermer"
                    >
                        ✕
                    </button>
                </div>

                <div className="p-8 overflow-y-auto custom-scrollbar">
                    <div className="grid grid-cols-3 gap-4 mb-8">
                        <div className="bg-slate-50 p-3 rounded-xl border border-slate-100">
                            <p className="text-[10px] uppercase text-slate-400 font-bold mb-1">Formateur</p>
                            <p className="text-sm font-semibold text-slate-700">{course.trainerFirstName} {course.trainerLastName}</p>
                        </div>
                        <div className="bg-slate-50 p-3 rounded-xl border border-slate-100">
                            <p className="text-[10px] uppercase text-slate-400 font-bold mb-1">Durée</p>
                            <p className="text-sm font-semibold text-slate-700">{course.durationInDays} jour(s)</p>
                        </div>
                        <div className="bg-slate-50 p-3 rounded-xl border border-slate-100">
                            <p className="text-[10px] uppercase text-slate-400 font-bold mb-1">Disponibilité</p>
                            <p className={`text-sm font-bold ${remainingPlaces <= 2 ? 'text-red-600' : 'text-emerald-600'}`}>
                                {remainingPlaces > 0 ? `${remainingPlaces} places` : 'Complet'}
                            </p>
                        </div>
                    </div>

                    <h4 className="font-bold text-slate-900 mb-3 flex items-center gap-2">
                        À propos de cette formation
                    </h4>

                    <div
                        className="prose prose-slate max-w-none text-slate-600 leading-relaxed
                       prose-headings:text-slate-900 prose-headings:font-bold
                       prose-li:marker:text-blue-500"
                        dangerouslySetInnerHTML={{ __html: course.longDescription || '<em>Pas de description détaillée.</em>' }}
                    />
                </div>

                <div className="p-6 border-t border-slate-100 bg-white flex items-center justify-between">
                    <div className="text-sm">
                        <span className="text-slate-400 italic">ID Session: {sessionId}</span>
                    </div>
                    <div className="flex gap-3">
                        <button
                            onClick={onClose}
                            className="px-6 py-2.5 text-slate-600 font-semibold hover:bg-slate-50 rounded-xl transition-colors"
                        >
                            Annuler
                        </button>
                        <button
                            onClick={handleRegisterClick}
                            className="bg-blue-600 hover:bg-blue-700 text-white px-8 py-2.5 rounded-xl font-bold shadow-lg shadow-blue-200 transition-all active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
                            disabled={remainingPlaces <= 0}
                        >
                            {remainingPlaces <= 0 ? 'Complet' : "S'inscrire"}
                        </button>
                    </div>
                </div>
            </div>
        </ModalOverlay>
    );
};

export default SessionDetailModal;