import React, { useState } from 'react';
import { useCreateCourse } from '../../../hooks/courses/useCreateCourse';
import { TargetAudience } from '../common/TargetAudience';

interface CreateCourseModalProps {
    onClose: () => void;
    onSuccess: () => void;
}

export const CreateCourseModal: React.FC<CreateCourseModalProps> = ({
    onClose,
    onSuccess
}) => {
    const { mutate: createCourse, loading, error } = useCreateCourse();
    const [formData, setFormData] = useState({
        name: '',
        shortDescription: '',
        longDescription: '',
        durationInDays: '',
        targetAudience: '',
        maxCapacity: '',
        trainerFirstName: '',
        trainerLastName: '',
    });

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSelectChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const result = await createCourse({
            name: formData.name,
            shortDescription: formData.shortDescription,
            longDescription: formData.longDescription,
            durationInDays: Number(formData.durationInDays),
            targetAudience: formData.targetAudience,
            maxCapacity: Number(formData.maxCapacity),
            trainerFirstName: formData.trainerFirstName,
            trainerLastName: formData.trainerLastName,
        });

        if (result) {
            onSuccess();
        }
    };

    // Style commun pour les inputs
    const inputStyle = "w-full border border-gray-300 rounded-lg px-3 py-2.5 text-sm focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none transition-all placeholder:text-gray-400";
    const labelStyle = "block text-sm font-semibold text-gray-700 mb-1.5";

    return (
        <div
            className="fixed inset-0 bg-slate-900/60 backdrop-blur-sm flex items-center justify-center z-50 p-4"
            onClick={onClose}
        >
            <div
                className="bg-white rounded-xl shadow-2xl w-full max-w-2xl max-h-[90vh] flex flex-col overflow-hidden"
                onClick={(e) => e.stopPropagation()}
            >
                {/* Header */}
                <div className="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
                    <h2 className="text-xl font-bold text-gray-800">Créer un nouveau cours</h2>
                    <button
                        onClick={onClose}
                        className="text-gray-400 hover:text-gray-600 transition-colors p-1"
                    >
                        <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>

                {/* Formulaire avec scroll si nécessaire */}
                <form onSubmit={handleSubmit} className="overflow-y-auto p-6 space-y-5">

                    {/* Section Titre */}
                    <div>
                        <label className={labelStyle}>Titre du cours *</label>
                        <input
                            type="text"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            required
                            className={inputStyle}
                        />
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div>
                            <label className={labelStyle}>Description courte</label>
                            <input
                                type="text"
                                name="shortDescription"
                                value={formData.shortDescription}
                                onChange={handleChange}
                                className={inputStyle}
                            />
                        </div>
                        <div>
                            <label className={labelStyle}>Population cible</label>
                            <select
                                name="targetAudience"
                                value={formData.targetAudience}
                                onChange={handleSelectChange}
                                className={inputStyle}
                                required
                            >
                                <option value="">Sélectionnez...</option>
                                {Object.values(TargetAudience).map((value) => (
                                    <option key={value} value={value}>
                                        {value === TargetAudience.CseElected ? "Élu CSE" : "Président CSE"}
                                    </option>
                                ))}
                            </select>
                        </div>

                    </div>

                    <div>
                        <label className={labelStyle}>Description longue</label>
                        <textarea
                            name="longDescription"
                            rows={3}
                            value={formData.longDescription}
                            onChange={handleChange}
                            className={`${inputStyle} resize-none`}
                        />
                    </div>

                    <div className="grid grid-cols-2 gap-4 p-4 bg-blue-50/50 rounded-lg">
                        <div>
                            <label className={labelStyle}>Durée (jours)</label>
                            <input
                                type="number"
                                name="durationInDays"
                                value={formData.durationInDays}
                                onChange={handleChange}
                                min="1"
                                className={inputStyle}
                            />
                        </div>
                        <div>
                            <label className={labelStyle}>Capacité max.</label>
                            <input
                                type="number"
                                name="maxCapacity"
                                value={formData.maxCapacity}
                                onChange={handleChange}
                                min="1"
                                className={inputStyle}
                            />
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4 border-t border-gray-100 pt-4">
                        <div>
                            <label className={labelStyle}>Prénom du formateur</label>
                            <input
                                type="text"
                                name="trainerFirstName"
                                value={formData.trainerFirstName}
                                onChange={handleChange}
                                className={inputStyle}
                            />
                        </div>
                        <div>
                            <label className={labelStyle}>Nom du formateur</label>
                            <input
                                type="text"
                                name="trainerLastName"
                                value={formData.trainerLastName}
                                onChange={handleChange}
                                className={inputStyle}
                            />
                        </div>
                    </div>

                    {error && (
                        <div className="p-3 bg-red-50 border border-red-200 text-red-600 text-sm rounded-lg flex items-center gap-2">
                            <span className="font-bold">⚠️</span> {error.message}
                        </div>
                    )}
                </form>

                {/* Footer avec boutons fixes */}
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
                        onClick={handleSubmit} // On peut déclencher le submit ici ou laisser le form s'en charger
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
                        ) : 'Créer le cours'}
                    </button>
                </div>
            </div>
        </div>
    );
};