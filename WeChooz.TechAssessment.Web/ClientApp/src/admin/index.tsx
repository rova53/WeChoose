
import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import './index.css'
import { CourseListPage } from "./pages/CourseListPage";
import { SessionListPage } from "./pages/SessionListPage";
import { UserListPage } from "./pages/UserListPage";

const root = ReactDOM.createRoot(document.getElementById("react-app") as HTMLElement);
root.render(
    <React.StrictMode>
        <BrowserRouter>
            <Routes>
                <Route path="/admin/courses" element={<CourseListPage />} />
                <Route path="/admin/sessions" element={<SessionListPage />} />
                <Route path="/admin/users" element={<UserListPage />} />
                <Route path="*" element={<Navigate to="/admin/courses" replace />} />
            </Routes>
        </BrowserRouter>
    </React.StrictMode>
);
