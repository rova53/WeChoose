import React from 'react';
import { TargetAudience, targetAudienceLabels } from '../../services/courses/TargetAudience';
import { DeliveryMode, deliveryModeLabels } from '../../services/sessions/DeliveryMode';
import { Paper, Title, Select, Radio, Group, Stack, Button, Divider, Text, TextInput } from '@mantine/core';
import { DatePickerInput } from '@mantine/dates';

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

    const handleSelectChange = (name: string, value: string | null) => {
        setFilters(prev => ({ ...prev, [name]: value || '' }));
    };

    const handleRadioChange = (value: string) => {
        setFilters(prev => ({ ...prev, mode: value }));
    };

    const handleDateChange = (name: string, date: Date | null) => {
        setFilters(prev => ({ ...prev, [name]: date ? date.toISOString() : '' }));
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
        <Paper p="md" shadow="xs" withBorder>
            <Group justify="space-between" mb="md">
                <Title order={4}>Filtres</Title>
                <Button variant="subtle" size="xs" onClick={resetFilters}>
                    Réinitialiser
                </Button>
            </Group>

            <Stack gap="md">
                {/* 1. Population Cible */}
                <Select
                    label="Population cible"
                    placeholder="Toutes les populations"
                    data={Object.values(TargetAudience)
                        .filter(v => typeof v === 'number')
                        .map((value) => ({
                            value: String(value),
                            label: targetAudienceLabels[value as TargetAudience]
                        }))}
                    value={filters.population}
                    onChange={(value) => handleSelectChange('population', value)}
                    clearable
                />

                {/* 2. Mode de délivrance */}
                <Radio.Group
                    label="Mode de délivrance"
                    value={filters.mode}
                    onChange={handleRadioChange}
                >
                    <Stack mt="xs" gap="xs">
                        {Object.values(DeliveryMode)
                            .filter(v => typeof v === 'number')
                            .map((value) => (
                                <Radio
                                    key={value}
                                    value={String(value)}
                                    label={deliveryModeLabels[value as DeliveryMode]}
                                />
                            ))}
                    </Stack>
                </Radio.Group>

                <Divider />

                {/* 3. Filtrage par Date */}
                <Select
                    label="Date de début"
                    placeholder="Peu importe la date"
                    data={[
                        { value: 'before', label: 'Avant le...' },
                        { value: 'after', label: 'Après le...' },
                        { value: 'between', label: 'Entre deux dates' }
                    ]}
                    value={filters.dateType}
                    onChange={(value) => handleSelectChange('dateType', value)}
                    clearable
                />

                {filters.dateType && (
                    <Stack gap="xs">
                        <TextInput
                            type="date"
                            value={filters.startDate ? filters.startDate.split('T')[0] : ''}
                            onChange={(e) => setFilters(prev => ({ ...prev, startDate: e.target.value }))}
                        />

                        {filters.dateType === 'between' && (
                            <>
                                <Text size="xs" ta="center" c="dimmed">et</Text>
                                <TextInput
                                    type="date"
                                    value={filters.endDate ? filters.endDate.split('T')[0] : ''}
                                    onChange={(e) => setFilters(prev => ({ ...prev, endDate: e.target.value }))}
                                />
                            </>
                        )}
                    </Stack>
                )}
            </Stack>
        </Paper>
    );
};

export default FilterSidebar;
