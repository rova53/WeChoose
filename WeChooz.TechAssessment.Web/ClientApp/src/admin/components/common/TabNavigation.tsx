import React from 'react';
import { Link, useLocation } from 'react-router-dom';

interface Tab {
    label: string;
    path: string;
    active?: boolean;
}

interface TabNavigationProps {
    tabs: Tab[];
}

export const TabNavigation: React.FC<TabNavigationProps> = ({ tabs }) => {
    const location = useLocation();

    return (
        <div className="border-b border-gray-200">
            <nav className="flex space-x-8" aria-label="Tabs">
                {tabs.map((tab) => {
                    // Vérification de l'état actif (soit via prop, soit via l'URL actuelle)
                    const isActive = tab.active ?? location.pathname === tab.path;

                    return (
                        <Link
                            key={tab.path}
                            to={tab.path}
                            className={`
                                relative py-4 px-1 text-sm font-medium transition-all duration-200 whitespace-nowrap
                                ${isActive
                                    ? 'text-blue-600'
                                    : 'text-gray-500 hover:text-gray-700 hover:border-gray-300'}
                            `}
                        >
                            {tab.label}

                            {/* Barre animée sous l'onglet actif */}
                            {isActive && (
                                <span className="absolute bottom-0 left-0 w-full h-0.5 bg-blue-600 rounded-full" />
                            )}
                        </Link>
                    );
                })}
            </nav>
        </div>
    );
};