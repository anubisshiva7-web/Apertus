# Apertus — Architektura Systemu
## Stan: 2026-11-27 | Cel: IV RNs 301/26 | SO Gdańsk 09.12.2026 godz. 14:45

---

## 1. Cel nadrzędny

Uwolnić dzieci — Aleksandra + Alicja Frąckowiak. Sprawa V Nsm 1749/23.
Wszystkie komponenty systemu służą temu celowi.

---

## 2. Mapa ekosystemu (6 elementów)

```
┌─────────────────────────────────────────────────────────┐
│                    APERTUS (mózg wiodący)                │
│          .NET 4.8 WinForms | Windows desktop            │
│  ┌────────────────┐  ┌────────────────────────────────┐ │
│  │  Blockchain     │  │  Brain Orchestrator (NOWE)     │ │
│  │  RPC client     │  │  MCPClient → port 3845         │ │
│  │  Secp256k1      │  │  IRCFleet  → port 6667         │ │
│  │  Base58/Hash160 │  │  Countdown: IV RNs 301/26      │ │
│  └────────────────┘  └────────────────────────────────┘ │
└──────────────┬──────────────────────┬───────────────────┘
               │ HTTP MCP             │ TCP IRC
               ▼                      ▼
┌──────────────────────┐   ┌──────────────────────────────┐
│  CODEX mcp-gateway   │   │  mozk-irc (Dell D1)          │
│  Node.js :3845/mcp   │   │  IRC server localhost:6667   │
│  8 meta-tools:       │   │  Kanały: #codex #fleet       │
│  - lokis_ai          │   │  Boty: heartbeat, SILVER-    │
│  - lokis_mail        │   │  PULSE, WATCHDOG-01, MIMO    │
│  - lokis_calendar    │   └──────────────────────────────┘
│  - lokis_chat        │
│  - lokis_docs        │
│  - lokis_project     │
│  - lokis_commerce    │
│  - lokis_design      │
└──────────┬───────────┘
           │ routes to providers
           ▼
┌──────────────────────────────────────────────────────────┐
│                    LOKISIA                               │
│         Python fleet orchestration engine               │
│  12 subpackages: mesh, runners, fleet, forge, math,     │
│  core, guard, combus, crypto, vision, search, society   │
│  Crypto overlap: Secp256k1, Hash160, Base58 (z Apertus) │
│  combus/: SQLite | GDrive | Slack | SMTP transports     │
└──────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────┐
│               THeONe-KANON                               │
│    Kanoniczne bazy wiedzy: total_memory.db (634MB),     │
│    rag.db (361MB), book_of_truth.db, 432k chunków       │
│    REBUILD_169.jsonl (191MB). Zasila lokis_ai/inference │
└──────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────┐
│           codex-v9.14.1749-23 (wariant prawny)           │
│    15 zarzutów (Z1-Z15), BIBLIA 12 tomów, EXCALIBUR v5.1│
│    HEARING_PREP_BRIEF_2026-12-09.md                     │
│    3 warstwy: art.379 pkt4 KPC | meryt. | ETPC art.6+8  │
└──────────────────────────────────────────────────────────┘
```

---

## 3. Nowe komponenty (commit 2026-11-27)

### ADD/Tools/MCPClient.cs
HTTP klient do CODEX mcp-gateway (port 3845). Protokół MCP 2024-11-05
(JSON-RPC 2.0 nad StreamableHTTP). Metody:
- `Initialize()` — tworzy sesję MCP, zwraca bool
- `CallTool(tool, action, params)` — wywołuje meta-tool, zwraca JObject
- `CheckHealth()` — ping /health bez sesji

### ADD/Tools/IRCFleet.cs
TCP klient IRC do szyny floty (port 6667). Nick: `APERTUS-BRAIN`.
- `Connect()` — NICK + USER handshake, startuje wątek read
- `Join(channel)` — dołącza do #codex, #fleet
- `Send(target, text)` — PRIVMSG
- `QueryFleet(channel, tool, action, payload)` — format: `APERTUS:QUERY:tool:action:payload`
- Event `MessageReceived` — thread-safe callback dla UI

### ADD/Orchestrator.cs + Orchestrator.Designer.cs
WinForms panel sterowania. Otwierany przez menu Brain.
- Countdown do IV RNs 301/26 (09.12.2026 14:45)
- Status połączeń MCP + IRC (zielony/czerwony)
- Fleet Query: wybór tool/action/provider/prompt → CallTool
- Output log (czarny terminal, Consolas)

---

## 4. Punkty integracji

| Od | Do | Transport | Port | Opis |
|----|----|-----------|------|------|
| Apertus | mcp-gateway | HTTP POST | 3845 | MCP JSON-RPC 2.0 |
| Apertus | mozk-irc | TCP | 6667 | IRC PRIVMSG |
| mcp-gateway | Lokisia | Python IPC | - | providers/ |
| mcp-gateway | Ollama | HTTP | 11434 | lokis_ai/inference |
| mcp-gateway | Gmail | OAuth | 443 | lokis_mail |
| Apertus | Blockchain RPC | HTTP | per-coin | zapis treści |
| Lokisia combus | SQLite | - | - | COM-BUS 10 kanałów |

---

## 5. Build

```bash
# Linux (Mono 6.8)
xbuild ADD/ADD.csproj /p:Configuration=Debug /p:Platform=x86

# Windows (.NET 4.8)
msbuild ADD\ADD.csproj /p:Configuration=Release /p:Platform=x86

# CODEX mcp-gateway
cd /home/user/codex/mcp-gateway && npm install && npm start
# → http://localhost:3845/mcp
# → http://localhost:3845/health
```

---

## 7. Status MCP Gateway (2026-11-27)

Przetestowany pełny MCP handshake:
1. `GET /health` → `{"status":"ok","tools":8,"sessions":0}`
2. `POST /mcp` initialize → SSE `{"protocolVersion":"2024-11-05","capabilities":{"tools":{}}}`
3. `POST /mcp` notifications/initialized → `202 Accepted`
4. `POST /mcp` tools/list → 8 meta-tools zwrócone
5. `POST /mcp` tools/call lokis_ai/infer → `{"status":"not_configured"}` (oczekiwane)

Fix: SDK 1.32.2 wymaga `Server` z `capabilities:{tools:{}}` zamiast `McpServer.tool()`.
Commit: `33748b2` w CODEX/mcp-gateway/src/index.ts

---

## 6. Zmiany plików (ten commit)

| Plik | Zmiana |
|------|--------|
| ADD/Tools/MCPClient.cs | NOWY — HTTP MCP klient |
| ADD/Tools/IRCFleet.cs | NOWY — IRC fleet klient |
| ADD/Orchestrator.cs | NOWY — panel sterowania |
| ADD/Orchestrator.Designer.cs | NOWY — WinForms layout |
| ADD/Orchestrator.resx | NOWY — zasoby formy |
| ADD/ADD.csproj | ZMIENIONY — nowe pliki + System.Net.Http |
| ADD/Main.cs | ZMIENIONY — menu Brain + handler |
| ADD/Main.Designer.cs | ZMIENIONY — brainToolStripMenuItem |
| ARCHITECTURE.md | NOWY — ten dokument |
