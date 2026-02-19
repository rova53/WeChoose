import React from 'react';
import { Group, TextInput, Button } from '@mantine/core';
import { IconSearch, IconPlus } from '@tabler/icons-react';

interface TableToolbarProps {
    search: string;
    onSearchChange: (value: string) => void;
    onAdd: () => void;
    searchPlaceholder?: string;
    addButtonLabel?: string;
}

export const TableToolbar: React.FC<TableToolbarProps> = ({
    search,
    onSearchChange,
    onAdd,
    searchPlaceholder = 'Rechercher...',
    addButtonLabel = 'Ajouter'
}) => {
    return (
        <Group justify="space-between" mb="md">
            <TextInput
                placeholder={searchPlaceholder}
                leftSection={<IconSearch size={16} />}
                value={search}
                onChange={(e) => onSearchChange(e.currentTarget.value)}
                style={{ flex: 1, maxWidth: 400 }}
            />
            <Button
                leftSection={<IconPlus size={18} />}
                onClick={onAdd}
            >
                {addButtonLabel}
            </Button>
        </Group>
    );
};
