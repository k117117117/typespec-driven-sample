export const API_URL =
  import.meta.env.VITE_API_URL || "http://localhost:5000";

export const SCHEMA_URL =
  import.meta.env.VITE_SCHEMA_URL || `${API_URL}/openapi.json`;
