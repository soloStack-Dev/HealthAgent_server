<div align="center">
  <br/>
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://img.shields.io/badge/%20-Medicine-6366f1?style=for-the-badge&logo=heart&logoColor=white">
    <img src="https://img.shields.io/badge/%20-Medicine-6366f1?style=for-the-badge&logo=heart&logoColor=white" height="36">
  </picture>

  <h1 align="center">HealthAgent API</h1>

  <p align="center">
    <strong>AI-Powered Health Symptom Analysis Engine</strong>
    <br/>
    Intelligent health recommendations powered by SLM (Phi-3.5) with medical-grade safety guardrails
  </p>

  <p align="center">
    <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8.0"/>
    <img src="https://img.shields.io/badge/Ollama-phi3.5-00FF00?style=flat-square&logo=ollama&logoColor=white" alt="Ollama"/>
    <img src="https://img.shields.io/badge/SQL_Server-EF_Core-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white" alt="SQL Server"/>
    <img src="https://img.shields.io/badge/Swagger-UI-85EA2D?style=flat-square&logo=swagger&logoColor=black" alt="Swagger"/>
    <img src="https://img.shields.io/badge/JWT-Bearer-000000?style=flat-square&logo=jsonwebtokens&logoColor=white" alt="JWT"/>
  </p>

  <br/>
</div>

---

## Overview

**HealthAgent API** is the backend engine for the HealthRec platform. It uses Microsoft **Phi-3.5** (a small language model running locally via Ollama) to analyze user-described symptoms, identify possible conditions, recommend actions, and surface trusted medical resources — all within strict safety guardrails.

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  React UI    │ ──▶ │  .NET 8 API  │ ──▶ │  Ollama/SLM  │ ──▶ │  Phi-3.5     │
│  (Client)    │ ◀── │  (Server)    │ ◀── │  (Local AI)  │     │  Model       │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
```

---

## Architecture

```
HealthAgent.Api/
├── Agents/                  # AI orchestration layer
│   ├── MediAgentKernel.cs   # Core agent: builds prompts, calls Ollama, parses output
│   ├── Plugins/             # AI plugins (safety, symptom analysis, web search)
│   ├── Prompts/             # 15 specialized prompt templates
│   └── Skills/              # Skill definitions (MD files used in system prompts)
├── Controllers/             # API controllers
├── Data/                    # EF Core DbContext + configurations
├── Endpoints/               # Minimal API endpoint definitions
├── Entities/                # Domain models (User, Conversation, Message, MedicalResource)
├── Infrastructure/          # Ollama HTTP client, web search service
├── Middleware/               # Exception handling + request logging
├── Models/                  # Request/Response DTOs
├── Services/                # Business logic (auth, conversations, health agent)
└── Common/                  # Shared utilities (ApiResponse, Result<T>, Constants)
```

---

## API Endpoints

### Authentication (`/api/auth`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Create a new account |
| `POST` | `/api/auth/login` | Sign in with email/password |
| `POST` | `/api/auth/refresh` | Refresh JWT token |

### Health Agent (`/api/health`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/health/analyze` | Submit symptoms for analysis |
| `POST` | `/api/health/chat` | Conversational symptom analysis |
| `GET` | `/api/health/suggestions` | Get symptom search suggestions |

### Conversations (`/api/conversations`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/conversations/` | List all user conversations |
| `GET` | `/api/conversations/{id}` | Get conversation details |
| `DELETE` | `/api/conversations/{id}` | Delete a conversation |
| `POST` | `/api/conversations/{id}/title` | Auto-generate conversation title |

### Resources (`/api/resources`)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/resources/search` | Search medical resources |
| `GET` | `/api/resources/trending` | Get trending health topics |

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Runtime** | .NET 8.0 (ASP.NET Core Minimal API) |
| **Database** | SQL Server via Entity Framework Core 8 |
| **Authentication** | JWT Bearer tokens (15-min expiry, refresh support) |
| **Password Hashing** | BCrypt.Net-Next (work factor 12) |
| **AI Model** | Phi-3.5 (Microsoft SLM) via Ollama |
| **API Docs** | Swagger / Swashbuckle |
| **CORS** | Configured for frontend dev servers |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Ollama](https://ollama.ai/) installed and running
- Phi-3.5 model pulled:
  ```bash
  ollama pull phi3.5
  ```
- SQL Server (LocalDB or full instance)

### Configuration

1. Update `appsettings.json` with your configuration:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=HealthAgentDb;Trusted_Connection=True;"
     },
     "Ollama": {
       "Endpoint": "http://localhost:11434",
       "Model": "phi3.5:latest",
       "TimeoutSeconds": 120,
       "MaxTokens": 2048,
       "Temperature": 0.3
     },
     "Jwt": {
       "Secret": "your-secret-key-min-32-chars",
       "Issuer": "HealthAgentApi",
       "Audience": "HealthAgentClient",
       "ExpiryMinutes": 15
     }
   }
   ```

### Run

```bash
# Apply migrations & start
dotnet run

# The API starts at https://localhost:5001
# Swagger UI: https://localhost:5001/swagger
```

---

## Safety & Ethics

This system implements strict safety guardrails:

- **No medical diagnosis** — The AI provides informational analysis only, never a definitive diagnosis
- **Emergency detection** — High-severity symptoms are flagged with urgent warnings
- **No medication recommendations** — The system explicitly avoids suggesting specific drugs
- **Verified resources** — Only links to trusted medical sources (WHO, Mayo Clinic, CDC, NHS, etc.)
- **Privacy first** — All health data is encrypted and never shared with third parties

> **Disclaimer:** This system is for informational purposes only. Always consult a qualified healthcare professional for medical decisions.

---

## Project Status

✅ Active Development — Core features complete, actively maintained.

<br/>
<hr/>

<div align="center">
  <sub>Built with care for better health insights.</sub>
</div>
