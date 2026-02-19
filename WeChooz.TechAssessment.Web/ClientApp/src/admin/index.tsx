import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { MantineProvider } from "@mantine/core";
import '@mantine/core/styles.layer.css';
import 'mantine-datatable/styles.layer.css';
import { CourseListPage } from "./pages/CourseListPage";
import { SessionListPage } from "./pages/SessionListPage";
import { UserListPage } from "./pages/UserListPage";
import { AuthGuard } from "./AuthGuard";
import { PolicyRoles } from "../services/auth/PolicyRoles";
import { RoleRedirect } from "./RoleRedirect";
import { AdminLayout } from "./AdminLayout";

const root = ReactDOM.createRoot(document.getElementById("react-app") as HTMLElement);
root.render(
    <React.StrictMode>
        <MantineProvider defaultColorScheme="light">
            <BrowserRouter>
                <Routes>
                    <Route path="/admin" element={<AdminLayout />}>
                        <Route path="courses" element={
                            <AuthGuard requiredRoles={[PolicyRoles.Formation]}><CourseListPage /></AuthGuard>
                        } />
                        <Route path="sessions" element={
                            <AuthGuard requiredRoles={[PolicyRoles.Formation]}><SessionListPage /></AuthGuard>
                        } />
                        <Route path="users" element={
                            <AuthGuard requiredRoles={[PolicyRoles.Sales]}><UserListPage /></AuthGuard>
                        } />
                    </Route>
                    
                    <Route path="/unauthorized" element={<div style={{ padding: '2rem', textAlign: 'center', color: '#c00', fontWeight: 'bold' }}>Accès refusé</div>} />
                    <Route path="*" element={<RoleRedirect />} />
                </Routes>
            </BrowserRouter>
        </MantineProvider>
    </React.StrictMode>
);
