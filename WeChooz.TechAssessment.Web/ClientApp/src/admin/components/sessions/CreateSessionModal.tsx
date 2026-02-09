import React, { useState } from 'react';
import { useCreateSession } from '../../../hooks/sessions/useCreateSession';
import { SessionDTO } from '../../../services/sessions/SessionDTO';

interface CreateSessionModalProps {
    onClose: () => void;
    onSuccess: () => void;
}

export const CreateSessionModal: React.FC<CreateSessionModalProps> = ({
    onClose,
    onSuccess
}) => {
    const { mutate: createSession, loading, error } = useCreateSession();
    const [formData, setFormData] = useState({
        courseId: '',
        courseName: '',
        startDate: '',
        deliveryMode: '',
        UserCount: 0,
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: name === 'UserCount' ? Number(value) : value
        }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        const result = await createSession(formData as Omit<SessionDTO, 'id'>);
        if (result) {
            onSuccess();
        }
    };

    const inputStyle = "w-full border border-gray-300 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition-all placeholder:text-gray-400";
    const labelStyle = "block text-sm font-semibold text-gray-700 mb-1.5";

    return (
        <div
            className="fixed inset-0 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center z-50 p-4"
            onClick={onClose}
        >
            <div
                className="bg-white rounded-xl shadow-2xl w-full max-w-xl max-h-[90vh] flex flex-col overflow-hidden"
                onClick={e => e.stopPropagation()}
            >
                {/* Header */}
                <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
                    <h2 className="text-xl font-bold text-gray-800">Créer une nouvelle session</h2>
                    <button
                        onClick={onClose}
                        className="text-gray-400 hover:text-gray-600 transition-colors p-1"
                    >
                        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="overflow-y-auto p-6 space-y-5">
                    <div>
                        <label className={labelStyle}>ID du cours *</label>
                        <input
                            type="text"
                            name="courseId"
                            value={formData.courseId}
                            onChange={handleChange}
                            required
                            className={inputStyle}
                        />
                    </div>
                    <div>
                        <label className={labelStyle}>Nom du cours *</label>
                        <input
                            type="text"
                            name="courseName"
                            value={formData.courseName}
                            onChange={handleChange}
                            required
                            className={inputStyle}
                        />
                    </div>
                    <div>
                        <label className={labelStyle}>Date de début *</label>
                        <input
                            type="date"
                            name="startDate"
                            value={formData.startDate}
                            onChange={handleChange}
                            required
                            className={inputStyle}
                        />
                    </div>
                    <div>
                        <label className={labelStyle}>Mode de diffusion *</label>
                        <input
                            type="text"
                            name="deliveryMode"
                            value={formData.deliveryMode}
                            onChange={handleChange}
                            required
                            className={inputStyle}
                        />
                    </div>
                    <div>
                        <label className={labelStyle}>Nombre de Users</label>
                        <input
                            type="number"
                            name="UserCount"
                            value={formData.UserCount}
                            onChange={handleChange}
                            min={0}
                            className={inputStyle}
                        />
                    </div>
                    {error && (
                        <div className="p-3 bg-red-50 border border-red-200 text-red-600 text-sm rounded-lg flex items-center gap-2">
                            <span className="font-bold">⚠️</span> {error.message}
                        </div>
                    )}
                </form>

                {/* Footer */}
                <div className="px-6 py-4 border-t border-gray-100 flex justify-end gap-3 bg-gray-50/50">
                    <button
                        type="button"
                        onClick={onClose}
                        disabled={loading}
                        className="px-5 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
                    >
                        Annuler
                    </button>
                    <button
                        type="submit"
                        onClick={handleSubmit}
                        disabled={loading}
                        className="px-5 py-2 text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 rounded-lg shadow-sm disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                    >
                        {loading ? (
                            <span className="flex items-center gap-2">
                                <svg className="animate-spin h-4 w-4 text-white" viewBox="0 0 24 24">
                                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                                </svg>
                                Création...
                            </span>
                        ) : 'Créer la session'}
                    </button>
                </div>
            </div>
        </div>
    );
};