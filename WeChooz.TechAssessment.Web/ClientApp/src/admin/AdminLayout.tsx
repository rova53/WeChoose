import React, { useEffect } from 'react';
import { AppShell, Group, Container, Tabs, Text, Button, Menu, Avatar, rem, Stack, Title } from '@mantine/core';
import { useNavigate, useLocation, Outlet } from 'react-router-dom';
import { IconLogout, IconChevronDown } from '@tabler/icons-react';
import { useCurrentUser } from '../hooks/auth/useCurrentUser';
import { PolicyRoles } from '../services/auth/PolicyRoles';

export const AdminLayout: React.FC = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { user, logout } = useCurrentUser();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    const hasRole = (role: PolicyRoles) => {
        return user?.roles !== undefined && (user.roles & role) === role;
    };

    const tabs = [
        { value: '/admin/courses', label: 'Cours', role: PolicyRoles.Formation },
        { value: '/admin/sessions', label: 'Sessions', role: PolicyRoles.Formation },
        { value: '/admin/users', label: 'Utilisateurs', role: PolicyRoles.Sales },
    ].filter(tab => hasRole(tab.role));

    const activeTab = tabs.find(t => location.pathname.startsWith(t.value))?.value || null;


    useEffect(() => {
        if (tabs.length > 0) {
            if (location.pathname === '/admin' || location.pathname === '/admin/') {
                navigate(tabs[0].value, { replace: true });
            } else if (!activeTab && location.pathname.startsWith('/admin')) {
                navigate(tabs[0].value, { replace: true });
            }
        }
    }, [location.pathname, tabs, activeTab, navigate]);

    return (
        <AppShell
            header={{ height: 60 }}
            padding="md"
            bg="gray.0"
        >
            <AppShell.Header>
                <Container size="lg" h="100%">
                    <Group h="100%" justify="space-between">
                        <Group>
                            <Text fw={900} size="xl" variant="gradient" gradient={{ from: 'blue', to: 'cyan', deg: 90 }}>
                                WeChooz
                            </Text>
                        </Group>



                        <Menu shadow="md" width={200} position="bottom-end">
                            <Menu.Target>
                                <Button variant="subtle" color="gray" rightSection={<IconChevronDown size={14} />}>
                                    <Group gap={7}>
                                        <Avatar src={null} alt={user?.username} radius="xl" size={24} color="blue">
                                            {user?.username?.charAt(0)}
                                        </Avatar>
                                        <Text fw={500} size="sm" lh={1} mr={3}>
                                            {user?.username}
                                        </Text>
                                    </Group>
                                </Button>
                            </Menu.Target>

                            <Menu.Dropdown>
                                <Menu.Label>Compte</Menu.Label>
                                <Menu.Item
                                    color="red"
                                    leftSection={<IconLogout style={{ width: rem(14), height: rem(14) }} />}
                                    onClick={handleLogout}
                                >
                                    Déconnexion
                                </Menu.Item>
                            </Menu.Dropdown>
                        </Menu>
                    </Group>
                </Container>
            </AppShell.Header>

            <AppShell.Main>
                <Container size="lg" pt="md" pb="xl">
                    <Stack gap="lg">
                        <Tabs
                            value={activeTab}
                            onChange={(value) => value && navigate(value)}
                            variant="outline"
                            radius="md"
                        >
                            <Tabs.List>
                                {tabs.map(tab => (
                                    <Tabs.Tab
                                        key={tab.value}
                                        value={tab.value}
                                        fw={600}
                                        px="lg"
                                    >
                                        {tab.label}
                                    </Tabs.Tab>
                                ))}
                            </Tabs.List>
                        </Tabs>

                        <div style={{ flex: 1 }}>
                            <Outlet />
                        </div>
                    </Stack>
                </Container>
            </AppShell.Main>
        </AppShell>
    );
};
