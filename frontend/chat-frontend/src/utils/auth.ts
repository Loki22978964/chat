import { jwtDecode } from "jwt-decode";

interface JwtPayload {
    exp: number;
}

export const isTokenValid = (token: string | null): boolean => {
    if (!token){
        return false;
    }

    try {
        const decoded = jwtDecode<JwtPayload>(token);
        const currentTime = Date.now() / 1000;

        return decoded.exp > currentTime;
    } catch( error){
        return false;
    }
};

export const getMyUserId = (): string | null => {
  const token = localStorage.getItem("token");
  if (!token) return null;
  try {
    const decoded: any = jwtDecode(token);
    return decoded.sub ?? decoded.nameid ?? null;
  } catch {
    return null;
  }
};