# CT-Solution — Documentazione Tecnica

> **Stack:** .NET 10 · ASP.NET Core Web API · DevExpress XPO (ORM) · SQLite (`AuthDatabase.db`)
>
> **Architettura:** Domain (entità + interfacce + DTO + value objects) · DataAccess (XPO entities + mapper + servizi) · Api (Controller + Program.cs)
>
> Tutte le date di entità usano un `Guid Id`. Le relazioni sono dichiarate tramite gli attributi `[Association("...")]` di XPO sui file `XpoEntities/*`; i `Domain/Entities/*` espongono le stesse navigazioni in stile POCO per l'uso nella business logic.

---

## 1 · Schema del Database (relazioni)

```
                                 ┌──────────────┐
                                 │  Permission  │
                                 │  Id (PK)     │
                                 │  Code        │
                                 └──────┬───────┘
                                        │  N : M  (Role-Permissions)
                                        │
┌──────────────┐   N : M (User-Roles)  │         ┌──────────────┐
│     User     ├───────────────────────►┤         │     Role     │
│  Id (PK)     │                       │         │  Id (PK)     │
│  Email       │                       └────────►│  Name        │
│  PasswordHash│                                 └──────┬───────┘
│  PasswordSalt│                                        │
└──────┬───────┘                                        │  1 : N  (User-Roles)
       │  1 : N  (User-Employees)                       │
       │                                                ▼
       ▼                                          (XpoUser.Roles)
┌──────────────┐
│   Employee   │
│  Id (PK)     │
│  UserName    │  1 : N (Employee-WorkLogs)
│  IdUser (FK) │
└──────┬───────┘
       │
       │  N : 1
       ▼
┌──────────────────┐
│    WorkLog       │  N : 1 ──► Project  (Project-WorkLogs)
│  Id (PK)         │  N : 1 ──► Employee (Employee-WorkLogs)
│  Description     │  N : 1 ──► Category (Category-WorkLogs)
│  HoursCounter    │  N : 1 ──► Status   (Status-WorkLogs)
│  Date            │
│  CreatedAt       │
│  UpdatedAt       │
│  IdProject (FK)  │
│  IdEmployee (FK) │
│  IdCategory (FK) │
│  IdStatus (FK)   │
└──────────────────┘

┌──────────────┐
│   Company    │  1 : N (Company-Projects)
│  Id (PK)     │
│  Name        │
│  Email       │
└──────┬───────┘
       │
       ▼
┌──────────────┐
│   Project    │
│  Id (PK)     │
│  Name        │
│  HourlyRate  │
│  IdCompany(FK)│
└──────────────┘
```

### 1.1 · Tabella riassuntiva delle relazioni

| Relazione | Tipo | Lato "1" | Lato "N" | Association name (XPO) |
|---|---|---|---|---|
| User ↔ Role | N : M | User | Role | `User-Roles` |
| Role ↔ Permission | N : M | Role | Permission | `Role-Permissions` |
| User → Employee | 1 : N | User | Employee | `User-Employees` |
| Employee → WorkLog | 1 : N | Employee | WorkLog | `Employee-WorkLogs` |
| Project → WorkLog | 1 : N | Project | WorkLog | `Project-WorkLogs` |
| Category → WorkLog | 1 : N | Category | WorkLog | `Category-WorkLogs` |
| Status → WorkLog | 1 : N | Status | WorkLog | `Status-WorkLogs` |
| Company → Project | 1 : N | Company | Project | `Company-Projects` |

> Tutte le FK sono `Guid` (non autoincrement). Niente cascade: i vincoli di integrità sono demandati ai service.

---

## 2 · Entità di dominio — a cosa servono e come si usano

### 2.1 `User`
**File:** `Domain/Entities/User.cs` · **Xpo:** `XpoUser.cs`

Rappresenta un account applicativo (chi può fare login). Conserva le credenziali (`PasswordHash` + `PasswordSalt` con PBKDF2-SHA256/100k iterazioni) e la lista dei `Role` assegnati. Un `User` può avere zero o più `Employee` collegati (es. account admin che possiede anche un profilo dipendente) e zero o più `Role`.

**Quando si usa:** autenticazione (`/api/user/login`, `/api/user/register`), gestione account, assegnazione ruoli (`/api/user/{userId}/roles/{roleId}`).

```csharp
var user = new User(
    id: Guid.NewGuid(),
    email: "mario@rossi.it",
    passwordHash: hash,
    passwordSalt: salt,
    roles: new List<Role> { userRole });
```

---

### 2.2 `Role`
**File:** `Domain/Entities/Role.cs` · **Xpo:** `XpoRole.cs`

Ruolo logico (`Admin`, `Manager`, `User`, …) al quale sono agganciate le `Permission`. Un utente riceve permessi **solo tramite il ruolo** (controllo a due livelli: ruolo → permessi). N : M sia verso `User` che verso `Permission`.

**Quando si usa:** creare ruoli (`POST /api/role/create`), comporre set di permessi (`POST /api/roles/{roleId}/permissions/{permissionId}`), verificare l'appartenenza.

```csharp
var role = new Role(Guid.NewGuid(), "Manager");
role.Permissions.Add(worklogWritePermission);
```

---

### 2.3 `Permission`
**File:** `Domain/Entities/Permission.cs` · **Xpo:** `XpoPermission.cs`

Permesso granulare espresso da un `Code` stringa (es. `"WorkLog.Create"`, `"Project.Delete"`). Non è legato a un endpoint: è un *token* che controller e service interpretano per abilitare o negare azioni. Assegnato ai `Role`, mai direttamente all'utente.

**Quando si usa:** creare la lista di codici di permesso (`POST /api/permission/create`), distribuirli nei ruoli, controllare se un utente può fare qualcosa (`GET /api/user/{userId}/permissions/check?permissionCode=...`).

```csharp
var p = new Permission(Guid.NewGuid(), "WorkLog.Create");
if (await _userService.HasPermissionAsync(userId, "WorkLog.Create", ct)) { ... }
```

---

### 2.4 `Employee`
**File:** `Domain/Entities/Employee.cs` · **Xpo:** `XpoEmployee.cs`

Profilo "anagrafico" del dipendente. Ha un `UserName` (es. `MRossi`) e, opzionalmente, un collegamento a un `User` (`IdUser`) per chi possiede anche un account applicativo. Un `Employee` registra più `WorkLog`.

**Quando si usa:** CRUD anagrafica dipendenti (`/api/employee/...`), punto di partenza per sapere "chi ha lavorato su cosa e quando" (si combina con `WorkLog`).

```csharp
var emp = new Employee(Guid.NewGuid(), "MRossi", userId);
```

---

### 2.5 `Company`
**File:** `Domain/Entities/Company.cs` · **Xpo:** `XpoCompany.cs`

Azienda cliente/committente. Ogni `Project` appartiene a una `Company`. Espone `Name` ed `Email` di riferimento.

**Quando si usa:** gestire l'anagrafica delle aziende per cui si fanno progetti; reportistica aggregata per cliente.

```csharp
var company = new Company(Guid.NewGuid(), "Acme SpA", "info@acme.it");
```

---

### 2.6 `Project`
**File:** `Domain/Entities/Project.cs` · **Xpo:** `XpoProject.cs`

Progetto lavorato, di proprietà di una `Company`. Ha un `Name` e un `HourlyRate` (tariffa oraria — usata nei calcoli di fatturazione/consuntivazione). Su un `Project` vengono registrati molti `WorkLog`.

**Quando si usa:** CRUD progetti (`/api/project/...`), agganciare `WorkLog` a un progetto, calcolare il consuntivo `ore × HourlyRate`.

```csharp
var p = new Project(Guid.NewGuid(), "Migrazione ERP", 75m, companyId);
```

---

### 2.7 `Category`
**File:** `Domain/Entities/Category.cs` · **Xpo:** `XpoCategory.cs`

Categoria di lavoro applicata a un `WorkLog` (es. *Sviluppo*, *Analisi*, *Bugfix*, *Riunione*). Serve a **tipizzare l'attività** per poi raggruppare/filtrarla in report.

**Quando si usa:** dropdown di selezione nei form di inserimento worklog, filtri di report ("ore totali per categoria").

```csharp
var cat = new Category(Guid.NewGuid(), "Sviluppo");
```

---

### 2.8 `Status`
**File:** `Domain/Entities/Status.cs` · **Xpo:** `XpoStatus.cs`

Stato del `WorkLog` (es. *Bozza*, *Inviato*, *Approvato*, *Fatturato*). È una **macchina a stati**: il `WorkLog` cambia stato seguendo il flusso approvativo/operativo.

**Quando si usa:** filtrare i worklog da approvare, dashboard operative ("quanti worklog in stato Bozza?").

```csharp
var s = new Status(Guid.NewGuid(), "Approvato");
```

---

### 2.9 `WorkLog`
**File:** `Domain/Entities/WorkLog.cs` · **Xpo:** `XpoWorkLog.cs`

**Entità centrale** del sistema: singola registrazione di ore lavorate. Contiene `Description`, `HoursCounter`, `Date` (solo giorno, `DateOnly`), timestamp di creazione/modifica, e quattro FK verso le entità di contesto:

- `IdProject` → su **quale progetto** ho lavorato
- `IdEmployee` → **chi** ha lavorato
- `IdCategory` → **che tipo** di attività era
- `IdStatus` → a **che punto** del workflow è

**Quando si usa:** CRUD worklog (`/api/worklog/...`), calcolo ore totali, export timesheet, fatturazione (`hours × project.HourlyRate`).

```csharp
var wl = new WorkLog(
    id: Guid.NewGuid(),
    description: "Refactor servizio pagamenti",
    hoursCounter: 4,
    date: DateOnly.FromDateTime(DateTime.Today),
    createdAt: DateTime.UtcNow,
    updatedAt: DateTime.UtcNow,
    idProject: projectId,
    idEmployee: employeeId,
    idCategory: categoryId,
    idStatus: statusId);
```

---

## 3 · Value Objects (`Domain/ValueObjects`)

| VO | Scopo |
|---|---|
| `ValueObject` | Base class per uguaglianza strutturale. |
| `EmailAddress` | Validazione/normalizzazione email (da usare quando si vuole centralizzare la logica). |
| `DisplayName` | Nome visualizzato (lunghezza, trim, …). |
| `Hours` | Wrapper su `int` per ore (regole di validazione > 0, max 24, …). |
| `Money` | Wrapper su `decimal` per importi/tariffe (arrotondamenti, valuta). |

> Sono dichiarati come primitive di dominio: i DTO odierni usano ancora i tipi CLR, ma i VO sono il punto di evoluzione naturale per le regole di validazione.

---

## 4 · Servizi (`DataAccess/Services`)

| Service | Responsabilità |
|---|---|
| `UserService` | Registrazione (con hash password + ruolo "User" di default), login (verifica + JWT), CRUD, assegnazione/rimozione ruoli, check ruolo/permesso. |
| `JwtService` | Genera il JWT (HS256, scadenza 2h) con claim `sub` = userId e `email`. Chiave hard-coded `super-secret-key-please-change-me` ⚠️. |
| `PasswordService` | PBKDF2-SHA256, salt 32 byte random, 100 000 iterazioni. |
| `RoleService` | CRUD dei ruoli. |
| `RolePermissionService` | Assegna/rimuove permessi a un ruolo, elenca permessi, check. |
| `PermissionService` | CRUD dei permessi. |
| `CompanyService` | CRUD aziende. |
| `ProjectService` | CRUD progetti. |
| `EmployeeService` | CRUD dipendenti. |
| `CategoryService` | CRUD categorie. |
| `StatusService` | CRUD stati. |
| `WorkLogService` | CRUD worklog (entità con più FK). |

Tutti i service usano il pattern `XpoDataContext.DoAsync` / `DoTranAsync` (commit solo in scrittura).

---

## 5 · API Endpoints

Base URL: `https://localhost:<port>/`

### 5.1 `POST /api/user/register`
Registra un nuovo utente. Crea automaticamente il ruolo `User` se assente.
- **Body:** `RegisterDto { Email, Password }`
- **Risposta:** `{ message: "Registration successful" }`

### 5.2 `POST /api/user/login`
Autentica e restituisce JWT.
- **Body:** `LoginDto { Email, Password }`
- **Risposta:** `AuthResponseDto { Token }`

### 5.3 `GET /api/user`
Lista di tutti gli utenti con relativi ruoli.

### 5.4 `GET /api/user/{id}`
Dettaglio utente per `Guid`.

### 5.5 `GET /api/user/email/{email}`
Dettaglio utente per email.

### 5.6 `POST /api/user/{userId}/roles/{roleId}`
Assegna un ruolo a un utente.

### 5.7 `DELETE /api/user/{userId}/roles/{roleId}`
Rimuove un ruolo da un utente.

### 5.8 `GET /api/user/{userId}/roles/check?roleName=Admin`
Restituisce `{ HasRole: bool }`.

### 5.9 `GET /api/user/{userId}/permissions/check?permissionCode=WorkLog.Create`
Restituisce `{ HasPermission: bool }` (verifica transitiva via ruoli).

---

### 5.10 `POST /api/role/create?name=Manager`
Crea un ruolo (`name` da query string).

### 5.11 `GET /api/role`
Lista ruoli.

### 5.12 `DELETE /api/role?roleId={guid}`
Elimina un ruolo.

---

### 5.13 `POST /api/permission/create?code=WorkLog.Create`
Crea un permesso (code da query string).

### 5.14 `GET /api/permission`
Lista permessi.

### 5.15 `DELETE /api/permission?permissionId={guid}`
Elimina un permesso.

---

### 5.16 `POST /api/roles/{roleId}/permissions/{permissionId}`
Assegna un permesso a un ruolo.

### 5.17 `DELETE /api/roles/{roleId}/permissions/{permissionId}`
Rimuove un permesso da un ruolo.

### 5.18 `GET /api/roles/{roleId}/permissions/check?permissionCode=WorkLog.Create`
Verifica se il ruolo possiede un permesso.

### 5.19 `GET /api/roles/{roleId}/permissions`
Elenca i permessi di un ruolo.

---

### 5.20 `POST /api/company/create`
Crea un'azienda.
- **Body:** `CreateCompanyDto { Name, Email }`

### 5.21 `GET /api/company`
### 5.22 `GET /api/company/{id}`

### 5.23 `PUT /api/company/update`
- **Body:** `UpdateCompanyDto { Id, Name, Email }`

### 5.24 `DELETE /api/company/delete?id={guid}`

---

### 5.25 `POST /api/project/create`
- **Body:** `CreateProjectDto { Name, HourlyRate, IdCompany }`

### 5.26 `GET /api/project`
### 5.27 `GET /api/project/{id}`

### 5.28 `PUT /api/project/update`
- **Body:** `UpdateProjectDto { Id, Name, HourlyRate, IdCompany }`

### 5.29 `DELETE /api/project/delete?id={guid}`

---

### 5.30 `POST /api/employee/create`
- **Body:** `CreateEmployeeDto { UserName, IdUser }` — `IdUser` collega (opzionalmente) un account.

### 5.31 `GET /api/employee`
### 5.32 `GET /api/employee/{id}`

### 5.33 `PUT /api/employee/update`
- **Body:** `UpdateEmployeeDto { Id, UserName }`

### 5.34 `DELETE /api/employee/delete?id={guid}`

---

### 5.35 `POST /api/category/create`
- **Body:** `CreateCategoryDto { Name }`

### 5.36 `GET /api/category`
### 5.37 `GET /api/category/{id}`

### 5.38 `PUT /api/category/update`
- **Body:** `UpdateCategoryDto { Id, Name }`

### 5.39 `DELETE /api/category/delete?id={guid}`

---

### 5.40 `POST /api/status/create`
- **Body:** `CreateStatusDto { Name }`

### 5.41 `GET /api/status`
### 5.42 `GET /api/status/{id}`

### 5.43 `PUT /api/status/update`
- **Body:** `UpdateStatusDto { Id, Name }`

### 5.44 `DELETE /api/status/delete?id={guid}`

---

### 5.45 `POST /api/worklog/create`
Crea un worklog.
- **Body:** `CreateWorkLogDto { Description, HoursCounter, Date, IdProject, IdEmployee, IdCategory, IdStatus }`

### 5.46 `GET /api/worklog`
### 5.47 `GET /api/worklog/{id}`

### 5.48 `PUT /api/worklog/update`
- **Body:** `UpdateWorkLogDto` (stessi campi + `Id`).

### 5.49 `DELETE /api/worklog/delete?id={guid}`

---

## 6 · Note operative

- **Auth lato server:** il JWT viene generato ma **non c'è ancora un middleware `[Authorize]`** sui controller. Da aggiungere prima dell'uso in produzione.
- **Segreto JWT** hard-coded in `JwtService.cs` — spostare in `appsettings.json` con `IConfiguration`.
- **Migrations:** lo schema è autogenerato da XPO (`AutoCreateOption.DatabaseAndSchema`). Adatto a sviluppo; per produzione preferire migration esplicite.
- **Permessi granulari vs ruoli hard-coded:** le `Permission` esistono ma non sono ancora applicate con attributi/filter; oggi la matrice ruolo→permesso è solo dichiarativa. L'endpoint `HasPermissionAsync` è già pronto per essere interrogato.
