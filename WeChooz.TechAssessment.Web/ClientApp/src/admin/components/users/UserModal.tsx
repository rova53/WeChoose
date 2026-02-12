import React, { useState, useEffect } from 'react';
import { useCreateUser } from '../../../hooks/users/useCreateUser';
import { useUpdateUser } from '../../../hooks/users/useUpdateUser'; // À créer si inexistant
import { UserDTO } from '../../../services/users/UserDTO';
import { useGetAllSessions } from '../../../hooks/sessions/useGetAllSessions';
import { SessionEnroll } from '../../../services/sessions/SessionEnrollDTO';

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

    console.log('user in modal:', user); // Debug
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

    // Styles Tailwind
    const inputStyle = "w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:ring-2 focus:ring-blue-500 outline-none transition-all";
    const labelStyle = "block text-sm font-semibold text-gray-700 mb-1.5";

    return (
        <div className="fixed inset-0 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center z-50 p-4" onClick={onClose}>
            <div className="bg-white rounded-xl shadow-2xl w-full max-w-xl max-h-[90vh] flex flex-col overflow-hidden" onClick={e => e.stopPropagation()}>

                {/* Header */}
                <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
                    <h2 className="text-xl font-bold text-gray-800">
                        {isEditMode ? `Modifier : ${user?.firstName} ${user?.lastName}` : 'Créer un utilisateur'}
                    </h2>
                    <button onClick={onClose} className="text-gray-400 hover:text-gray-600 p-1">
                        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" /></svg>
                    </button>
                </div>

                {/* Formulaire */}
                <form id="user-form" onSubmit={handleSubmit} className="overflow-y-auto p-6 space-y-5">
                    <div className="grid grid-cols-2 gap-4">
                        <div>
                            <label className={labelStyle}>Nom *</label>
                            <input type="text" name="lastName" value={formData.lastName} onChange={handleChange} required className={inputStyle} />
                        </div>
                        <div>
                            <label className={labelStyle}>Prénom *</label>
                            <input type="text" name="firstName" value={formData.firstName} onChange={handleChange} required className={inputStyle} />
                        </div>
                    </div>

                    <div>
                        <label className={labelStyle}>Email *</label>
                        <input type="email" name="email" value={formData.email} onChange={handleChange} required className={inputStyle} />
                    </div>

                    {!isEditMode && (
                        <div>
                            <label className={labelStyle}>Mot de passe *</label>
                            <input type="password" name="password" value={formData.password} onChange={handleChange} required className={inputStyle} />
                        </div>
                    )}

                    <div>
                        <label className={labelStyle}>Entreprise</label>
                        <input type="text" name="companyName" value={formData.companyName} onChange={handleChange} className={inputStyle} />
                    </div>

                    {/* Section Sessions */}
                    <div>
                        <label className={labelStyle}>Inscrire à des sessions</label>
                        <div className="border border-gray-200 rounded-lg bg-gray-50/30 max-h-40 overflow-y-auto p-1">
                            {sessions.map(session => (
                                <label key={session.id} className="flex items-center gap-3 px-3 py-2 hover:bg-blue-50 cursor-pointer rounded-md transition-colors">
                                    <input
                                        type="checkbox"
                                        checked={selectedSessions.includes(session.id)}
                                        onChange={() => handleCheckboxChange(session.id)}
                                        className="w-4 h-4 text-blue-600"
                                    />
                                    <div className="flex flex-col">
                                        <span className="text-sm font-medium">{session.courseName}</span>
                                        <span className="text-xs text-gray-500">{new Date(session.startDate).toLocaleDateString()}</span>
                                    </div>
                                </label>
                            ))}
                        </div>
                    </div>

                    {error && <div className="p-3 bg-red-50 text-red-600 text-sm rounded-lg">! {error.message}</div>}
                </form>

                {/* Footer */}
                <div className="px-6 py-4 border-t border-gray-100 flex justify-end gap-3 bg-gray-50/50">
                    <button type="button" onClick={onClose} className="px-5 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">Annuler</button>
                    <button form="user-form" type="submit" disabled={loading} className="px-5 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg shadow-sm disabled:opacity-50">
                        {loading ? 'Traitement...' : isEditMode ? 'Enregistrer' : 'Créer l\'utilisateur'}
                    </button>
                </div>
            </div>
        </div>
    );
};