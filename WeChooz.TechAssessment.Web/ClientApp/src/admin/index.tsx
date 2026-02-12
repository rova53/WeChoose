import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import './index.css'
import { CourseListPage } from "./pages/CourseListPage";
import { SessionListPage } from "./pages/SessionListPage";
import { UserListPage } from "./pages/UserListPage";
import { AuthGuard } from "./AuthGuard";
import { PolicyRoles } from "../services/auth/PolicyRoles";
import { RoleRedirect } from "./RoleRedirect";

const root = ReactDOM.createRoot(document.getElementById("react-app") as HTMLElement);
root.render(
    <React.StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/admin/courses" element=
                    {
                        <AuthGuard requiredRoles={[PolicyRoles.Formation]}><CourseListPage /></AuthGuard>
                    } />
                <Route path="/admin/sessions" element={
                    <AuthGuard requiredRoles={[PolicyRoles.Formation]}><SessionListPage /></AuthGuard>
                } />
                <Route path="/admin/users" element={<AuthGuard requiredRoles={[PolicyRoles.Sales]}><UserListPage /></AuthGuard>} />
                <Route path="/unauthorized" element={<div style={{ padding: '2rem', textAlign: 'center', color: '#c00', fontWeight: 'bold' }}>Accès refusé</div>} />
                <Route path="*" element={<RoleRedirect />} />
            </Routes>
        </BrowserRouter>
    </React.StrictMode>
);
