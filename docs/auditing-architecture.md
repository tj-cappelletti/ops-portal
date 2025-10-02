# Auditing Architecture

## Overview
The audit system implements a multi-layered approach to capture all system activity while maintaining performance and configurability.

## Core Components

### 1. MediatR Pipeline Behavior
- **Purpose**: Capture user intent through commands
- **When**: Pre and post command execution
- **What**: Command type, user, timestamp, success/failure

### 2. EF Core Interceptors
- **Purpose**: Track actual data changes
- **When**: During SaveChanges
- **What**: Entity changes, old/new values

### 3. Domain Event Auditing
- **Purpose**: Business-level events
- **When**: After domain events raised
- **What**: Event type, aggregate state

### 4. Query Auditing
- **Purpose**: Track data access
- **When**: During sensitive queries
- **What**: Query parameters, user, results count

## Configuration Levels

| Level | Homelab | Small Business | Enterprise |
|-------|---------|----------------|------------|
| Commands | ✓ | ✓ | ✓ |
| Data Changes | - | Key Only | All |
| Queries | - | Sensitive | All |
| Retention | 7 days | 90 days | 7 years |
| External SIEM | - | - | ✓ |

## Performance Considerations
- Async logging prevents blocking main operations
- Configurable detail levels reduce overhead
- Batch processing for high-volume scenarios
- Separate read models for audit queries

## Compliance Features
- Immutable audit records
- Cryptographic signatures (optional)
- Automated retention policies
- Export capabilities for external systems

## Overall Audit Architecture
```mermaid
graph TB
    subgraph "User Actions"
        U[User Request]
    end
    
    subgraph "API Layer"
        C[Controller]
        MW[Audit Middleware]
    end
    
    subgraph "Application Layer"
        MED[MediatR Pipeline]
        CB[Command/Query Bus]
        AB[Audit Behavior]
        H[Handler]
    end
    
    subgraph "Domain Layer"
        E[Entity]
        DE[Domain Events]
    end
    
    subgraph "Infrastructure Layer"
        EF[EF Core]
        INT[Audit Interceptor]
        DB[(Database)]
    end
    
    subgraph "Audit System"
        AS[Audit Service]
        AH1[Command Audit Handler]
        AH2[Data Change Handler]
        AH3[Event Audit Handler]
        AH4[Query Audit Handler]
        AL[(Audit Log)]
    end
    
    U --> C
    C --> MW
    MW --> MED
    MED --> AB
    AB --> CB
    CB --> H
    H --> E
    E --> DE
    E --> EF
    EF --> INT
    INT --> DB
    
    MW -.-> AS
    AB -.-> AH1
    INT -.-> AH2
    DE -.-> AH3
    CB -.-> AH4
    
    AH1 --> AL
    AH2 --> AL
    AH3 --> AL
    AH4 --> AL
    
    style AS fill:#f9f,stroke:#333,stroke-width:4px
    style AL fill:#bbf,stroke:#333,stroke-width:2px
```

## Command Audit FLow with MediatR Pipeline
```mermaid
sequenceDiagram
    participant User
    participant Controller
    participant MediatR
    participant AuditBehavior
    participant Handler
    participant AuditService
    participant Database

    User->>Controller: POST /api/solution-stacks
    Controller->>MediatR: Send(CreateSolutionStackCommand)
    
    Note over MediatR: Pipeline Behaviors Execute
    MediatR->>AuditBehavior: Pre-Handler Execution
    
    AuditBehavior->>AuditBehavior: Create AuditContext
    Note over AuditBehavior: Capture: User, IP, Timestamp,<br/>Command Type, Request Data
    
    AuditBehavior->>Handler: Continue Pipeline
    Handler->>Database: Save Entity
    Database-->>Handler: Success/Failure
    Handler-->>AuditBehavior: Response/Exception
    
    alt Success
        AuditBehavior->>AuditBehavior: Mark Success
        Note over AuditBehavior: Add: Duration, Response Data
    else Failure
        AuditBehavior->>AuditBehavior: Mark Failed
        Note over AuditBehavior: Add: Error Details
    end
    
    AuditBehavior-->>AuditService: LogAsync(AuditEntry)
    Note over AuditService: Async - doesn't block
    AuditService-->>Database: Insert Audit Log
    
    AuditBehavior-->>MediatR: Return Response
    MediatR-->>Controller: Response
    Controller-->>User: HTTP Response
```

## Entity Framework Core Change Tracking Audit
```mermaid
graph LR
    subgraph "Entity State Tracking"
        CT[Change Tracker]
        E1[Added Entities]
        E2[Modified Entities]
        E3[Deleted Entities]
    end
    
    subgraph "Audit Interceptor"
        SI[SavingChanges Event]
        CE[Capture Changes]
        CV[Compare Values]
        AE[Create Audit Entries]
    end
    
    subgraph "Save Process"
        SC[SaveChanges]
        DB[(Database)]
        SA[SavedChanges Event]
    end
    
    subgraph "Audit Storage"
        AS[Audit Service]
        AL[(Audit Log)]
    end
    
    CT --> E1
    CT --> E2
    CT --> E3
    
    E1 --> SI
    E2 --> SI
    E3 --> SI
    
    SI --> CE
    CE --> CV
    CV --> AE
    
    AE --> SC
    SC --> DB
    DB --> SA
    SA --> AS
    AS --> AL
    
    style CT fill:#ffd,stroke:#333,stroke-width:2px
    style AS fill:#f9f,stroke:#333,stroke-width:2px
```

## Unified Audit Trail Correlation
```mermaid
graph TB
    subgraph "Single User Operation"
        OP[User Updates Solution Stack]
    end
    
    subgraph "Audit Events Generated"
        C[Command Audit<br/>UpdateSolutionStackCommand]
        D[Data Change Audit<br/>Entity Modified]
        E[Domain Event Audit<br/>SolutionStackUpdated]
        Q[Query Audit<br/>GetSolutionStackById]
    end
    
    subgraph "Correlation"
        CID[Correlation ID:<br/>abc-123-def]
    end
    
    subgraph "Unified View"
        UV[Unified Audit Entry]
        TL[Timeline View]
        IM[Impact Analysis]
    end
    
    OP --> C
    OP --> D
    OP --> E
    OP --> Q
    
    C --> CID
    D --> CID
    E --> CID
    Q --> CID
    
    CID --> UV
    UV --> TL
    UV --> IM
    
    style CID fill:#bbf,stroke:#333,stroke-width:4px
    style UV fill:#dfd,stroke:#333,stroke-width:2px
```

## Deployment-Specific Audit Levels
```mermaid
graph LR
    subgraph "Homelab Configuration"
        H1[Auth Events ✓]
        H2[Commands ✓]
        H3[Data Changes ✗]
        H4[Queries ✗]
        H5[7 Day Retention]
    end
    
    subgraph "Small Business"
        S1[Auth Events ✓]
        S2[Commands ✓]
        S3[Key Data Changes ✓]
        S4[Sensitive Queries ✓]
        S5[90 Day Retention]
    end
    
    subgraph "Enterprise"
        E1[All Auth Events ✓]
        E2[All Commands ✓]
        E3[All Data Changes ✓]
        E4[All Queries ✓]
        E5[7 Year Retention]
        E6[External SIEM ✓]
    end
    
    H1 --> H5
    H2 --> H5
    
    S1 --> S5
    S2 --> S5
    S3 --> S5
    S4 --> S5
    
    E1 --> E5
    E2 --> E5
    E3 --> E5
    E4 --> E5
    E5 --> E6
    
    style H5 fill:#fdd,stroke:#333,stroke-width:2px
    style S5 fill:#ffd,stroke:#333,stroke-width:2px
    style E5 fill:#dfd,stroke:#333,stroke-width:2px
```

## Audit Service Decision Flow
```mermaid
flowchart TD
    Start[Audit Event Received]
    
    Check1{Auditing<br/>Enabled?}
    Check2{Event Type?}
    Check3{Is Excluded<br/>Action?}
    Check4{Is Always<br/>Audit Action?}
    Check5{Sensitivity<br/>Level?}
    
    Skip[Skip Audit]
    Min[Minimal Audit]
    Std[Standard Audit]
    Full[Full Audit]
    
    Store[Store in Database]
    External[Send to SIEM]
    Alert[Raise Alert]
    
    Start --> Check1
    Check1 -->|No| Skip
    Check1 -->|Yes| Check4
    
    Check4 -->|Yes| Full
    Check4 -->|No| Check3
    
    Check3 -->|Yes| Skip
    Check3 -->|No| Check2
    
    Check2 -->|Security| Full
    Check2 -->|Command| Check5
    Check2 -->|Query| Check5
    Check2 -->|Data Change| Std
    
    Check5 -->|Public| Min
    Check5 -->|Internal| Std
    Check5 -->|Confidential| Full
    
    Min --> Store
    Std --> Store
    Full --> Store
    Full --> External
    Full --> Alert
    
    style Skip fill:#fdd,stroke:#333,stroke-width:2px
    style Full fill:#dfd,stroke:#333,stroke-width:2px
    style Alert fill:#f9f,stroke:#333,stroke-width:4px
```

## Audit Data Lifecycle
```mermaid
stateDiagram-v2
    [*] --> Created: User Action
    
    Created --> Enriched: Add Context
    note right of Enriched
        - User Info
        - IP Address
        - Correlation ID
        - Timestamp
    end note
    
    Enriched --> Stored: Save to Database
    
    Stored --> Active: < 90 days
    Active --> Archived: > 90 days
    
    Active --> Analyzed: Reporting
    Active --> Alerted: Suspicious Activity
    
    Archived --> Compressed: Storage Optimization
    Compressed --> ColdStorage: Long-term Retention
    
    ColdStorage --> Restored: Compliance Request
    Restored --> Active
    
    ColdStorage --> Deleted: Retention Expired
    Deleted --> [*]
    
    Alerted --> Investigated: Security Team
    Investigated --> Resolved
    Resolved --> Stored
```

## Performance Impact Visualization
```mermaid
graph TB
    subgraph "Without Auditing"
        R1[Request] --> H1[Handler: 50ms] --> RS1[Response]
    end
    
    subgraph "With Async Auditing"
        R2[Request] --> AB2[Audit: 2ms] --> H2[Handler: 50ms] --> AB3[Audit: 3ms] --> RS2[Response]
        AB3 -.-> AS2[Async Store: 20ms]
    end
    
    subgraph "With Sync Auditing"
        R3[Request] --> AB4[Audit: 2ms] --> H3[Handler: 50ms] --> AB5[Audit: 25ms] --> RS3[Response]
    end
    
    Note1[Total: 50ms]
    Note2[Total: 55ms<br/>+10% overhead]
    Note3[Total: 77ms<br/>+54% overhead]
    
    RS1 --> Note1
    RS2 --> Note2
    RS3 --> Note3
    
    style AS2 fill:#dfd,stroke:#333,stroke-width:2px
    style Note2 fill:#dfd,stroke:#333,stroke-width:2px
    style Note3 fill:#fdd,stroke:#333,stroke-width:2px
```