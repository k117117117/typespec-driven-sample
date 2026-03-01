import createClient from "openapi-fetch";
import type { paths } from "./generated/admin";
import { API_URL } from "./config";

export const apiClient = createClient<paths>({ baseUrl: API_URL });
