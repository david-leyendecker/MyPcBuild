# **Software Design Document: My PC Build**

## **1\. Executive Summary**

"My PC Build" is a multi-user web application and PWA designed for enthusiasts to plan, document, and share custom PC configurations. The system utilizes a lightweight Event Sourcing architecture to track the evolution of PC builds over time.

## **2\. Architectural Goals & Constraints**

* **Polymorphic Data:** Components share common traits but have unique technical specs.  
* **Global Catalog:** A shared repository of products identified by GUIDs.  
* **Lightweight Event Sourcing:** Build state derived from an immutable event stream.  
* **Mobile-First PWA:** Priority on touch-friendly UI and offline capabilities.  
* **Utility-First UI:** Shift from Radzen to a more flexible Tailwind-driven architecture.

## **3. Tech Stack (Current)**

* **Orchestration:** .NET Aspire (orchestrates API, PostgreSQL, and Vite dev server)  
* **Backend:** ASP.NET Core 10 Minimal APIs  
* **Frontend:** Vue.js 3 (TypeScript, Composition API)
* **UI Framework:** PrimeVue 4 component library
* **Styling:** PrimeFlex utility-first CSS
* **State Management:** Pinia
* **Routing:** Vue Router
* **Build Tool:** Vite
* **Database & Event Store:** PostgreSQL with Marten for event sourcing and projections

## **4\. System Architecture**

### **4.1 Event-Sourced Architecture**

The system records "Commands" which result in "Events":

1. **Command:** AddPart(BuildId, ProductId, Price)  
2. **Event Store:** Immutable events persisted via Marten in PostgreSQL.  
3. **Projections:** Background projections update the read model in PostgreSQL.

## **5\. Data Model & Categories**

* **Product Categories:** CPU, Motherboard, GPU, RAM, **PC Case**, PSU.  
* **Read Projection:** Materialized view (PostgreSQL/Marten) including compatibilityIssues with Error or Warning severities.

## **6\. Visual Design & UX Strategy (PrimeFlex + PrimeVue)**

* **Component-Based:** Using PrimeVue for UI components (Dialogs, Buttons, Cards, Forms).  
* **Utility-Driven:** Using PrimeFlex utility classes for layouts, spacing, and responsive design.  
* **Mobile-First UX:** Responsive grid layouts using PrimeFlex grid system and breakpoint utilities.

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
* **Transactional Outbox:** Atomic writes via PostgreSQL.

## **9\. PWA & Frontend Strategy**

* **Reactive State:** Pinia stores manage application state with Vue reactivity.
* **Optimistic UI:** Local updates via Pinia stores, followed by API sync.  
* **Type Safety:** Full TypeScript support for compile-time safety.
* **Component Composition:** Vue 3 Composition API for better code reusability.

## **10\. API Design (Event-Centric)**

* GET /api/catalog/search \- Search global products.  
* POST /api/builds/{id}/parts \- Add a part.  
* DELETE /api/builds/{id}/parts/{productId} \- Remove a part.

## **11\. Deployment (Aspire)**

* **Infrastructure:** .NET Aspire managing containerized services.  
* **Storage:** PostgreSQL (JSON and relational) via Marten.