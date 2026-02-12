import React from 'react';
import { TargetAudience, targetAudienceLabels } from '../../services/courses/TargetAudience';
import { DeliveryMode, deliveryModeLabels } from '../../services/sessions/DeliveryMode';

// Interface pour typer les filtres
export interface SessionFilters {
    population: string;
    mode: string;
    dateType: 'before' | 'after' | 'between' | '';
    startDate: string;
    endDate: string;
}

interface FilterSidebarProps {
    filters: SessionFilters;
    setFilters: React.Dispatch<React.SetStateAction<SessionFilters>>;
}

const FilterSidebar = ({ filters, setFilters }: FilterSidebarProps) => {

    const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value } = e.target;
        setFilters(prev => ({ ...prev, [name]: value }));
    };

    const resetFilters = () => {
        setFilters({
            population: '',
            mode: '',
            dateType: '',
            startDate: '',
            endDate: ''
        });
    };

    return (
        <aside className="w-full md:w-64 bg-white p-6 rounded-xl shadow-sm border border-gray-100 h-fit sticky top-6">
            <div className="flex justify-between items-center mb-6">
                <h2 className="font-bold text-gray-800 text-lg">Filtres</h2>
                <button
                    onClick={resetFilters}
                    className="text-xs text-blue-600 hover:underline"
                >
                    Réinitialiser
                </button>
            </div>

            <div className="space-y-6">
                {/* 1. Population Cible */}
                <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                        Population cible
                    </label>
                    <select
                        name="population"
                        value={filters.population}
                        onChange={handleChange}
                        className="w-full p-2 bg-gray-50 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-blue-500 outline-none"
                    >
                        <option value="">Toutes les populations</option>
                        {Object.values(TargetAudience)
                            .filter(v => typeof v === 'number')
                            .map((value) => (
                                <option key={value} value={value}>
                                    {targetAudienceLabels[value as TargetAudience]}
                                </option>
                            ))}
                    </select>
                </div>

                {/* 2. Mode de délivrance */}
                <div>
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                        Mode de délivrance
                    </label>
                    <div className="space-y-2">
                        {Object.values(DeliveryMode)
                            .filter(v => typeof v === 'number')
                            .map((value) => (
                                <label key={value} className="flex items-center text-sm cursor-pointer group">
                                    <input
                                        type="radio"
                                        name="mode"
                                        value={value}
                                        checked={filters.mode === String(value)}
                                        onChange={handleChange}
                                        className="w-4 h-4 text-blue-600 border-gray-300 focus:ring-blue-500"
                                    />
                                    <span className="ml-2 text-gray-600 group-hover:text-gray-900">
                                        {deliveryModeLabels[value as DeliveryMode]}
                                    </span>
                                </label>
                            ))}
                    </div>
                </div>

                {/* 3. Filtrage par Date */}
                <div className="pt-4 border-t border-gray-100">
                    <label className="block text-sm font-semibold text-gray-700 mb-2">
                        Date de début
                    </label>

                    <select
                        name="dateType"
                        value={filters.dateType}
                        onChange={handleChange}
                        className="w-full p-2 mb-3 bg-gray-50 border border-gray-200 rounded-lg text-sm"
                    >
                        <option value="">Peu importe la date</option>
                        <option value="before">Avant le...</option>
                        <option value="after">Après le...</option>
                        <option value="between">Entre deux dates</option>
                    </select>

                    {filters.dateType && (
                        <div className="space-y-3 animate-in slide-in-from-top-1 duration-200">
                            <input
                                type="date"
                                name="startDate"
                                value={filters.startDate}
                                onChange={handleChange}
                                className="w-full p-2 border border-gray-200 rounded-lg text-sm"
                            />

                            {filters.dateType === 'between' && (
                                <>
                                    <span className="text-xs text-gray-400 block text-center">et</span>
                                    <input
                                        type="date"
                                        name="endDate"
                                        value={filters.endDate}
                                        onChange={handleChange}
                                        className="w-full p-2 border border-gray-200 rounded-lg text-sm"
                                    />
                                </>
                            )}
                        </div>
                    )}
                </div>
            </div>
        </aside>
    );
};

export default FilterSidebar;