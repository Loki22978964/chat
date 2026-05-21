import { useEffect, useRef } from "react";
import * as signalR from "@microsoft/signalr";

export function useSignalR(
  chatId: string | null,
  onMessage: (msg: any) => void,
) {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const chatIdRef = useRef<string | null>(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5052/chatHub", {
        accessTokenFactory: () => localStorage.getItem("token") ?? "",
        transport: signalR.HttpTransportType.WebSockets,
        skipNegotiation: true,
      })
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveMessage", onMessage);

    connection.onreconnected(() => {
      if (chatIdRef.current) {
        connection.invoke("JoinChat", chatIdRef.current).catch(console.error);
      }
    });

    connection
      .start()
      .then(() => {
        if (chatIdRef.current) {
          connection.invoke("JoinChat", chatIdRef.current).catch(console.error);
        }
      })
      .catch((err) => console.error("❌ SignalR connection error:", err));

    connectionRef.current = connection;

    return () => {
      connection.stop();
    };
  }, []);

  useEffect(() => {
    const connection = connectionRef.current;
    const prevChatId = chatIdRef.current;

    if (
      prevChatId &&
      connection?.state === signalR.HubConnectionState.Connected
    ) {
      connection.invoke("LeaveChat", prevChatId).catch(console.error);
    }

    chatIdRef.current = chatId;

    if (chatId && connection?.state === signalR.HubConnectionState.Connected) {
      connection.invoke("JoinChat", chatId).catch(console.error);
    }
  }, [chatId]);
}
