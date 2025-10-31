# 🧩 Fuse-Inventory

**Fuse-Inventory** is a lightweight, self-hosted service inventory and permissions tracker.  
It’s designed to be **“useful in 30 minutes”** — drop it into a container, describe your services, and immediately see how everything fits together.

---

## ✨ Overview

Fuse-Inventory lets small development or DevOps teams:

- Track **services**, their **hosts**, and **dependencies**
- Record **databases, accounts, and grants**
- Import and export data as **YAML or JSON**
- Validate and **reverse-import** live SQL permissions
- Generate **SQL GRANT / REVOKE** scripts for restores
- Compare **environments** and see drift at a glance
- Run entirely from a **single Docker container**

It’s inspired by tools like **NetBox** and **Uptime-Kuma**, but stripped down for simplicity:
no external database, no multi-service stack — just one binary and a data file.

---

## 🚀 Quick Start

```bash
docker run -d \
  --name fuse-inventory \
  -p 8080:8080 \
  -v fuse-inventory-data:/data \
  ghcr.io/youruser/fuse-inventory:latest
```
Then open http://localhost:8080
 in your browser.

Fuse-Inventory stores all data in /data/data.json by default —
mount that volume to back it up or share between upgrades.

🛠️ Development
Prerequisites

.NET 8 SDK

Node 20+ & npm

Docker (optional)

Clone & run
```bash
git clone https://github.com/youruser/fuse-inventory.git
cd fuse-inventory
```

1️⃣ Run backend (API)
```bash
dotnet run --project src/Server --urls http://localhost:5180
```

2️⃣ Run frontend (Vue + Vite dev server)
```bash
cd src/Web
npm install
npm run dev
```

Open http://localhost:5180
 — the .NET app proxies SPA requests to Vite,
so you get full hot-module reload and API access through one port.

🧰 Build for production
```bash
# build Vue assets
npm --prefix src/Web run build

# publish .NET app (includes SPA)
dotnet publish src/Server -c Release -o build
```

Docker build
```bash
docker build -t fuse-inventory -f docker/Dockerfile .
docker run -d -p 8080:8080 -v fuse-inventory-data:/data fuse-inventory
```


📦 Roadmap

 SQL Server reverse-import (auto-discover permissions)

 Environment drift reports

 Service health checks

 Key Vault integration (store secret URIs)

 Diff-based change sets / approvals

 YAML schema validation

 Export templates (SQL, CSV, Terraform)

📜 License

MIT License — see LICENSE
 for details.