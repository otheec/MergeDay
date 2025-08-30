type AnyObj = Record<string, any>;

const RUNTIME: AnyObj =
  typeof window !== "undefined" && (window as any).RUNTIME_CONFIG
    ? (window as any).RUNTIME_CONFIG
    : {};

const VITE: AnyObj =
  typeof import.meta !== "undefined" && (import.meta as any).env
    ? (import.meta as any).env
    : {};

const get = (key: string, def?: string) =>
  (RUNTIME[key] as string | undefined) ??
  (VITE[`VITE_${key}`] as string | undefined) ??
  def;

export const env = {
  MODE: (VITE.MODE as string) || "production",
  DEV: Boolean(VITE.DEV),
  PROD: Boolean(VITE.PROD),

  API_URL: get("API_URL", "/api"),
} as const;

if (env.DEV && !env.API_URL) {
  console.warn("[env] Missing API_URL; using fallback '/api'");
}
