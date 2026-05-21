const BASE_URL = "http://localhost:5052";

export const registerUser = async (
    username: string,
    email: string,
    password: string
) => {
    const res = await fetch(`${BASE_URL}/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({username, email, password}),
    });

    if(!res.ok){
        throw new Error("Registration failed");
    }
};

export const loginUser  = async (
    email: string,
    password: string
) => {
    const res = await fetch(`${BASE_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json"},
        body: JSON.stringify({email, password}),
    });

    if(!res.ok){
        throw new Error("Login failed");
    }
    

    const data = await res.json();
    
    return data.token;
};