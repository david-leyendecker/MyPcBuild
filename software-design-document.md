# **Software Design Document: My PC Build**

## **1\. Executive Summary**

"My PC Build" is a multi-user web application and PWA designed for enthusiasts to plan, document, and share custom PC configurations. The system utilizes a lightweight Event Sourcing architecture to track the evolution of PC builds over time.

## **2\. Architectural Goals & Constraints**

* **Polymorphic Data:** Components share common traits but have unique technical specs.  
* **Global Catalog:** A shared repository of products identified by GUIDs.  
* **Lightweight Event Sourcing:** Build state derived from an immutable event stream.  
* **Mobile-First PWA:** Priority on touch-friendly UI and offline capabilities.  
* **Utility-First UI:** Shift from Radzen to a more flexible Tailwind-driven architecture.

## **3\. Tech Stack (Updated)**

* **Orchestration:** .NET Aspire (.NET 10\)  
* **Backend:** ASP.NET Core Minimal APIs  
* **Frontend:** Blazor WebAssembly \+ **PrimeBlazor** (PrimeVue equivalent for Blazor)  
* **Styling:** **Tailwind CSS** for custom layouts and Glassmorphism effects.  
* **Database:** FerretDB (Postgres backend)

## **4\. System Architecture**

### **4.1 Event-Sourced Architecture**

The system records "Commands" which result in "Events":

1. **Command:** AddPart(BuildId, ProductId, Price)  
2. **Event Store:** Stores immutable records.  
3. **Projections:** Background service updates the "Read Model" in FerretDB.

## **5\. Data Model & Categories**

* **Product Categories:** CPU, Motherboard, GPU, RAM, **PC Case**, PSU.  
* **Read Projection:** Materialized view including compatibilityIssues with Error or Warning severities.

## **6\. Visual Design & UX Strategy (Tailwind \+ PrimeBlazor)**

* **Component-Based:** Using PrimeBlazor for high-level components (Dialogs, Auto-complete, Toast).  
* **Utility-Driven:** Using Tailwind CSS for the specific "Midnight" and "Glassmorphism" aesthetics that Radzen struggled to support natively.  
* **Mobile-First UX:** Custom bottom-pill navigation built with Tailwind flexbox/grid.

## **7\. Visual Reference & Component Mockup**

### **7.1 Layout & Structure**

* **Ambient Background:** Deep charcoal base with soft radial gradients.  
* **Sticky "Glass" Header:** Contains build name and the Compatibility Shield.  
* **Floating Pill Navigation:** Centered bar at bottom for "Add", "Log", and "Specs".

## **8\. Advanced Architectural Enhancements**

### **8.1 Multi-Level Compatibility Engine**

* **Severity: Error:** Incompatible Sockets, RAM Generation, Form Factor mismatch (e.g., ATX board in ITX Case).  
* **Severity: Warning:** GPU/Cooler clearance limits (within 10% threshold), header mismatches.

### **8.2 Consistency & Concurrency**

* **Version Tracking:** Every event/projection includes a version number.  
* **Transactional Outbox:** Atomic writes via FerretDB/PostgreSQL.

## **9\. PWA & Frontend Strategy**

* **Optimistic UI:** Local updates via Blazor state, followed by background event sync.  
* **Notification System:** PrimeBlazor ToastService for real-time compatibility alerts.

## **10\. API Design (Event-Centric)**

* GET /api/catalog/search \- Search global products.  
* POST /api/builds/{id}/parts \- Add a part.  
* DELETE /api/builds/{id}/parts/{productId} \- Remove a part.

## **11\. Deployment (Aspire)**

* **Infrastructure:** .NET Aspire managing containerized services.  
* **Storage:** PostgreSQL/FerretDB for hybrid JSON/Relational storage.