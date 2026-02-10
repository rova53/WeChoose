import { TargetAudience, targetAudienceLabels } from '../../services/courses/TargetAudience';
import { DeliveryMode, deliveryModeLabels } from '../../services/sessions/DeliveryMode';
import { SessionDTO } from '../../services/sessions/SessionDTO';

interface SessionCardProps {
    session: SessionDTO;
    onViewDetails: () => void;
}

const SessionCard = ({ session, onViewDetails }: SessionCardProps) => {
    console.log("session dans SessionCard:", session);
    // Formater la date proprement
    const formattedDate = new Date(session.startDate).toLocaleDateString('fr-FR', {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
    });

    return (
        <div className="bg-white border border-gray-200 rounded-xl overflow-hidden shadow-sm hover:shadow-md transition-shadow duration-300 flex flex-col sm:flex-row">
            {/* Indicateur visuel latéral selon le mode */}
            <div className={`w-2 ${session.deliveryMode === DeliveryMode.InPerson ? 'bg-indigo-500' : 'bg-emerald-500'}`} />

            <div className="p-5 flex-1">
                <div className="flex justify-between items-start mb-2">
                    <div>
                        <span className="text-xs font-semibold uppercase tracking-wider text-indigo-600 bg-indigo-50 px-2 py-1 rounded">
                            {targetAudienceLabels[session.course.targetAudience as TargetAudience] ?? session.course.targetAudience}
                        </span>
                        <h3 className="text-xl font-bold text-gray-900 mt-2">{session.courseName}</h3>
                    </div>
                    <div className="text-right">
                        <span className={`text-sm font-bold ${session.course.maxCapacity <= 3 ? 'text-red-500' : 'text-gray-500'}`}>
                            {session.userCount} places restantes
                        </span>
                    </div>
                </div>

                <p className="text-gray-600 text-sm line-clamp-2 mb-4">
                    {session.course.shortDescription}
                </p>

                <div className="grid grid-cols-2 md:grid-cols-4 gap-4 py-4 border-t border-gray-100 text-sm text-gray-700">
                    <div className="flex flex-col">
                        <span className="text-gray-400 text-xs uppercase">Date</span>
                        <span className="font-medium">{formattedDate}</span>
                    </div>
                    <div className="flex flex-col">
                        <span className="text-gray-400 text-xs uppercase">Durée</span>
                        <span className="font-medium">{session.course.durationInDays} jour(s)</span>
                    </div>
                    <div className="flex flex-col">
                        <span className="text-gray-400 text-xs uppercase">Mode</span>
                        <span className="font-medium">
                            {deliveryModeLabels[session.deliveryMode as DeliveryMode] ?? session.deliveryMode}
                        </span>
                    </div>
                    <div className="flex flex-col">
                        <span className="text-gray-400 text-xs uppercase">Formateur</span>
                        <span className="font-medium">{session.course.trainerLastName}</span>
                    </div>
                </div>
            </div>

            <div className="bg-gray-50 p-5 flex items-center justify-center border-t sm:border-t-0 sm:border-l border-gray-100">
                <button
                    onClick={onViewDetails}
                    className="w-full sm:w-auto px-6 py-2.5 bg-white border border-indigo-600 text-indigo-600 font-semibold rounded-lg hover:bg-indigo-600 hover:text-white transition-colors duration-200 shadow-sm"
                >
                    Voir détails
                </button>
            </div>
        </div>
    );
};

export default SessionCard;