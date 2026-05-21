import { useState } from "react";
import { loginUser, registerUser } from "./api/authApi";

interface AuthScreenProps {
  onLoginSuccess: () => void;
}

export const AuthScreen = ({ onLoginSuccess }: AuthScreenProps) => {
  const [isLogin, setLogin] = useState(true);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [dpassword, setDuplicate] = useState("");
  const [name, setName] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (!isLogin && password !== dpassword) {
      setError("Passwords don't match");
    }

    setLoading(true);

    try {
      if (isLogin) {
        const token = await loginUser(email, password);
        localStorage.setItem("token", token);
        onLoginSuccess(); // ← тільки при успіху
      } else {
        await registerUser(name, email, password);
        setLogin(true);
      }
    } catch (err: any) {
      setError(err.message || "Щось пішло не так");
    } finally {
      setLoading(false);
    }

    onLoginSuccess();
  };

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#f5f5f3] px-4">
      <div className="text-center mb-12">
        <h1 className="text-3xl text-[#2c2c2c] mb-2">Chat</h1>
        <p className="text-[#6b6b6b]">Simple, clean conversations</p>
      </div>

      <div className="bg-white rounded-3xl shadow-sm p-8 md:p-12">
        <div className="flex flex-col gap-2 mb-8">
          <div className="flex gap-3 mb-8">
            <button
              onClick={() => setLogin(true)}
              className={`flex-1 py-3 px-4 rounded-full transition-colors ${
                isLogin
                  ? "bg-[#3d5a80] text-white"
                  : "bg-transparent text-[#6b6b6b] hover:bg-[#f5f5f3]"
              }`}
            >
              Log in
            </button>
            <button
              onClick={() => setLogin(false)}
              className={`flex-1 py-3 px-4 rounded-full transition-colors ${
                !isLogin
                  ? "bg-[#3d5a80] text-white"
                  : "bg-transparent text-[#6b6b6b] hover:bg-[#f5f5f3]"
              }`}
            >
              Register
            </button>
          </div>

          <form onSubmit={handleSubmit} className="space-y-5 flex flex-col">
            {!isLogin && (
              <div>
                <label
                  htmlFor="name"
                  className="block text-sm text-[#6b6b6b] mb-2"
                >
                  Name
                </label>

                <input
                  id="name"
                  type="text"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="w-full px-5 py-3.5 bg-[#f5f5f3] border-none rounded-full focus:outline-none focus:ring-2 focus:ring-[#3d5a80]/20"
                  placeholder="Your name"
                  required={!isLogin}
                />
              </div>
            )}
            <div>
              <label
                htmlFor="email"
                className="block text-sm text-[#6b6b6b] mb-2"
              >
                email
              </label>

              <input
                id="email"
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full px-5 py-3.5 bg-[#f5f5f3] border-none rounded-full focus:outline-none focus:ring-2 focus:ring-[#3d5a80]/20"
                placeholder="you@example.com"
                required
              />
            </div>

            <div>
              <label
                htmlFor="password"
                className="block text-sm text-[#6b6b6b] mb-2"
              >
                pass
              </label>
              <input
                id="password"
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full px-5 py-3.5 bg-[#f5f5f3] border-none rounded-full focus:outline-none focus:ring-2 focus:ring-[#3d5a80]/20"
                placeholder="******"
                required
              />
            </div>

            {!isLogin && (
              <div>
                <label
                  htmlFor="password"
                  className="block text-sm text-[#6b6b6b] mb-2"
                >
                  pass
                </label>
                <input
                  id="confirm-password"
                  type="password"
                  value={dpassword}
                  onChange={(e) => setDuplicate(e.target.value)}
                  className="w-full px-5 py-3.5 bg-[#f5f5f3] border-none rounded-full focus:outline-none focus:ring-2 focus:ring-[#3d5a80]/20"
                  placeholder="******"
                  required
                />
              </div>
            )}
            <button
              type="submit"
              className="
    w-full py-4 rounded-2xl font-semibold text-white
    bg-[#3d5a80] 
    
    hover:ring-2 hover:ring-[#3d5a80] hover:ring-offset-2 
    hover:bg-[#2e4566]
    
    active:scale-[0.98] 
    
    transition-all duration-200
  "
            >
              {isLogin ? "Log in" : "Create account"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};
