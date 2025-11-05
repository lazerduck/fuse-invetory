# 🧩 Fuse-Inventory
<img width="512" height="512" alt="Fuse-inventory" src="https://github.com/user-attachments/assets/2b4cd430-ed63-4f62-af85-3e89470bd0aa" />

**Fuse-Inventory** is a lightweight, self-hosted service inventory and permissions tracker.
It’s designed to be self-contained, fast, and frictionless — letting you get set up in seconds.

---

## ✨ Overview

Fuse-Inventory lets small to medium sized development or DevOps teams:

- Track **applications**, their **hosts**, and **dependencies**
- Record **databases, accounts, and permissions**
- Present clear, living documentation of your application setup
- Import and export data as **YAML or JSON**
- Validate and **reverse-import** live SQL permissions
- Generate **SQL GRANT / REVOKE** scripts for restores
- Compare **environments** and see drift at a glance
- Run entirely from a **single Docker container**

It treats applications as first class objects with servers, dependencies and environments expected to facilitate applications.

## 🧩 Current Status
Fuse-Inventory is in early development.
The core data model and API are established, providing a foundation for the upcoming interface and automation features.

🛣️ Roadmap
- UI
  - Interactive forms
  - Config and relationship visualisation
- Security
  - Authentication & roles
  - Encrypted secret storage
- Deployments
  - Container image and version tracking
  - Environment promotion flows
- Support
  - Schema documentation
  - Data import/export tools
