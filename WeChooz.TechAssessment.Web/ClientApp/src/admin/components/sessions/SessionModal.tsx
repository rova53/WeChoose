import React, { useState, useEffect } from 'react';
import { useCreateSession } from '../../../hooks/sessions/useCreateSession';
import { useUpdateSession } from '../../../hooks/sessions/useUpdateSession';
import { useGetAllCourses } from '../../../hooks/courses/useGetAllCourses';
import { SessionDTO } from '../../../services/sessions/SessionDTO';
import { DeliveryMode, deliveryModeLabels } from '../../../services/sessions/DeliveryMode';


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
        deliveryMode: '' as string | number, // Ajusté pour accepter l'initialisation vide
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

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;

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
                deliveryMode: Number(value) // Convertit la chaîne du select en nombre pour l'Enum
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

    const inputStyle = "w-full border border-gray-300 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition-all disabled:bg-gray-100 disabled:cursor-not-allowed";
    const labelStyle = "block text-sm font-semibold text-gray-700 mb-1.5";

    return (
        <div className="fixed inset-0 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center z-50 p-4" onClick={onClose}>
            <div className="bg-white rounded-xl shadow-2xl w-full max-w-xl max-h-[90vh] flex flex-col overflow-hidden" onClick={e => e.stopPropagation()}>

                {/* Header */}
                <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
                    <h2 className="text-xl font-bold text-gray-800">
                        {isEditMode ? `Modifier la session` : 'Créer une nouvelle session'}
                    </h2>
                    <button onClick={onClose} className="text-gray-400 hover:text-gray-600 p-1">
                        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                <form id="session-form" onSubmit={handleSubmit} className="overflow-y-auto p-6 space-y-5">

                    {!isEditMode ? (
                        <div>
                            <label className={labelStyle}>Sélectionner le cours *</label>
                            <select
                                name="courseId"
                                value={formData.courseId}
                                onChange={handleChange}
                                required
                                className={inputStyle}
                                disabled={loadingCourses}
                            >
                                <option value="">{loadingCourses ? 'Chargement...' : '--- Choisir un cours ---'}</option>
                                {courses.map(course => (
                                    <option key={course.id} value={course.id}>
                                        {course.name}
                                    </option>
                                ))}
                            </select>
                        </div>
                    ) : (
                        <div>
                            <label className={labelStyle}>Cours</label>
                            <input
                                type="text"
                                value={formData.courseName}
                                disabled
                                className={inputStyle}
                            />
                        </div>
                    )}

                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className={labelStyle}>Date de début *</label>
                            <input type="date" name="startDate" value={formData.startDate} onChange={handleChange} required className={inputStyle} />
                        </div>
                        <div>
                            <label className={labelStyle}>Mode de diffusion *</label>
                            <select
                                name="deliveryMode"
                                value={formData.deliveryMode}
                                onChange={handleChange}
                                required
                                className={inputStyle}
                            >
                                <option value="">Sélectionner</option>
                                {Object.keys(deliveryModeLabels).map((key) => (
                                    <option key={key} value={key}>
                                        {deliveryModeLabels[Number(key) as DeliveryMode]}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>

                    <div>
                        <label className={labelStyle}>Nombre de participants maximum</label>
                        <input type="number" name="UserCount" value={formData.UserCount} onChange={handleChange} min={0} className={inputStyle} />
                    </div>

                    {error && (
                        <div className="p-3 bg-red-50 text-red-600 text-sm rounded-lg flex items-center gap-2">
                            <span>⚠️</span> {error.message}
                        </div>
                    )}
                </form>

                {/* Footer */}
                <div className="px-6 py-4 border-t border-gray-100 flex justify-end gap-3 bg-gray-50/50">
                    <button type="button" onClick={onClose} disabled={loading} className="px-5 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">
                        Annuler
                    </button>
                    <button
                        form="session-form"
                        type="submit"
                        disabled={loading || (!isEditMode && !formData.courseId)}
                        className="px-5 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg shadow-sm disabled:opacity-50 transition-all"
                    >
                        {loading ? 'Traitement...' : isEditMode ? 'Enregistrer' : 'Créer'}
                    </button>
                </div>
            </div>
        </div>
    );
};