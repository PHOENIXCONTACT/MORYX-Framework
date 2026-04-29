<!-- markdownlint-disable MD024 -->
# Feature Specification - Material Management Module

The Material Management Module provides a tailored control of the digital twins of material containers in a cyber-physical system.
It manages:

* linkage of information to other domain concepts, such as products or parts within a container, the orders it belongs to or the remainder of the cyber-physical system;
* operations on the material containers, such as their creation in, or removal from, the system, the adjustments to the filling level, ...;
* process-related constraints for operations on the material containers;
* monitoring and auditing of material (container) related information within the system.

## Specifications

### [SPEC-MMM-001] Define custom material container types

**Status:** Proposed\
**Persona:** Application Engineer, Key User\
**Goal:** Integrate material containers with application-specific properties and handling

#### System Behavior

* **Requirement 1:** Custom material container definitions can have properties that serve application-specific purposes.
* **Requirement 2:** Custom material container definitions can react to external triggers in application-specific ways.
* **Requirement 3:** Container types represent physical container kinds (e.g., barrels, boxes, trays) and may add optional, type-dependent properties.

---

### [SPEC-MMM-002] Register material container in the system

**Status:** Proposed\
**Persona:** Operator, Application Engineer\
**Goal:** Register existing, physical material containers to be known to the application

#### System Behavior

* **Requirement 1:** A physical material container can be registered in the application with a unique, scannable identifier using [Identities](../abstractions/index.md#identity).
* **Requirement 2:** Depending on the container type, different information may be required for registration.
**Failure Handling:** Missing information for the container registration or a failure to persist the information must be presented to the user.

#### Constraints

* **Constraint 1:** Operators might need to regularly register new material containers in the application requiring this process to be easy, fast, and guided.

---

### [SPEC-MMM-003] Search material containers

**Status:** Proposed\
**Persona:** Operator, Application Engineer\
**Goal:** Find material containers by providing unique identifiers as search parameters

#### System Behavior

* **Requirement 1:** Support inputs for container [Identities](../abstractions/index.md#identity) (e.g. QR codes, Barcode).
* **Requirement 2:** Support inputs for order numbers.
* **Requirement 3:** Support inputs for product identifiers.
* **Requirement 4:** Resolve IDs to current state and metadata; return clear results for display in the visual interface.
**Failure Handling:** Unknown, duplicate, or inactive IDs yield clear errors and operator guidance.

---

### [SPEC-MMM-004] Link containers to domain constructs and capture metadata

**Status:** Proposed\
**Persona:** Operator, Application Engineer, Key User\
**Goal:** Create and maintain links between containers and domain constructs with configurable metadata.

#### System Behavior

* **Requirement 1:** Support link types based on container type: order-container link, operator-container link, product-container link, and product instance-container link.
* **Requirement 2:** When given a container and a domain construct information, create/update a link between the container and the domain construct.
* **Requirement 3:** Capture configurable metadata with defaults; examples: order number, product identifier/name, quantity, operator code, workplace, raw material number/name, storage location, tool, container type, date, plant/site, machine/station.
* **Requirement 4:** Containers must be referenceable from the plant's digital twin ([Resources](../module-resources/index.md)), enabling contextual embedding by plant, workplace, and station.
* **Requirement 5:** Product (instance) link integrates with [Products](../module-products/index.md).
* **Requirement 6:** Operator link integrates with Operators.
**Failure Handling:** Validation errors prevent linking and are shown with actionable messages.

#### Constraints

* **Constraint 1:** Only link information appropriate to the [container type](#spec-mmm-001-define-custom-material-container-types).
* **Constraint 2:** Product/material links are subject to type constraints (see [SPEC-MMM-005](#spec-mmm-005-validate-links-by-typed-container-constraints)).

---

### [SPEC-MMM-005] Validate links by typed container constraints

**Status:** Proposed\
**Persona:** Application Engineer, Operator\
**Goal:** Ensure only valid container types are linked for a given product.

#### System Behavior

* **Requirement 1:** Type-specific properties on product-linked containers can define controlled vocabularies (e.g., hole-size classes 1.2 mm/2.0 mm).
* **Requirement 2:** Constraints are defined on container types using controlled vocabularies for relevant properties.
* **Requirement 3:** Constraints are evaluated before linking; violations reject the action with a clear message.

#### Constraints

* **Constraint 1:** Existing constraint types are maintainable via configuration/visual interface without code changes and can vary by container type.
* **Constraint 2:** They are enforced via the [filling/linking hooks](#spec-mmm-009-fillinglinking-hooks-and-blocking-policies).
* **Constraint 3:** Standard hooks are enabled by default and can be disabled per container type and per instance.

---

### [SPEC-MMM-006] Display linked details on visual interface

**Status:** Proposed\
**Persona:** Operator, Service Technician\
**Goal:** Show the currently linked details and metadata when inputting container information at any station.

#### System Behavior

* **Requirement 1:** Input of a container identity or link information returns all linked details and metadata.
**Failure Handling:** If no current link exists, clearly indicate empty/available state.

---

### [SPEC-MMM-007] Corrections with acknowledgment

**Status:** Proposed\
**Persona:** Operator, Key User, Service Technician\
**Goal:** Allow corrections to link data with configurable acknowledgment in critical contexts.

#### System Behavior

* **Requirement 1:** Users can edit metadata, links and linked details.
* **Requirement 2:** Operations (configurable by action, type or instance) can require explicit acknowledgment: operator identity, timestamp, and optional reason are recorded.
* **Requirement 3:** A standard constraint to wire this acknowledgment to a [linking/filling change hook](#spec-mmm-009-fillinglinking-hooks-and-blocking-policies) is available.
**Failure Handling:** Missing acknowledgment blocks the change.

### [SPEC-MMM-008] Container lineage: split/merge and auto-unlink on refill

**Status:** Proposed\
**Persona:** Operator, Application Engineer\
**Goal:** Track material movement between containers with traceable lineage and sensible defaults.

#### System Behavior

* **Requirement 1:** Split: one source container → multiple destination containers; propagate/aggregate metadata as configured; source can be unlinked as part of the operation.
* **Requirement 2:** Merge: multiple source containers → one destination container; preserve lineage for backward/forward traceability.
* **Requirement 3:** Unlink: explicit end of a container–order/product link.
* **Requirement 4:** Auto-unlink on refill is the default behavior for refill operations and can be governed by hooks (see [SPEC-MMM-009](#spec-mmm-009-fillinglinking-hooks-and-blocking-policies)).
**Failure Handling:** Inconsistent lineage operations (e.g., missing sources) are rejected with clear feedback.

---

### [SPEC-MMM-009] Filling/Linking hooks and blocking policies

**Status:** Proposed\
**Persona:** Application Engineer, Operator\
**Goal:** Provide configurable hooks around linking, unlinking, and quantity changes to enforce policies and prevent mix-ups.

#### System Behavior

* **Requirement 1:** Provide hooks for: before-link, after-link, before-unlink, after-unlink, before-quantity-change, after-quantity-change.
* **Requirement 2:** Hooks can block an operation, require acknowledgment, or allow it; they can modify or enrich metadata when allowed.
* **Requirement 3:** Hooks are configurable per container type and per container instance.
* **Requirement 4:** Configuration can be modified via visual interface or within code.
* **Requirement 5:** A standard hook is provided by the module that blocks linking if a container is still linked (prevents double-linking) unless disabled per configuration.
* **Requirement 6:** A standard hook is provided by the module that requires operator acknowledgment when unlinking/relinking in "critical areas" of the cyber-physical system.

#### Constraints

* **Constraint 1:** Acknowledgment requirements integrate with [SPEC-MMM-007](#spec-mmm-007-corrections-with-acknowledgment).

---

### [SPEC-MMM-010] Counting: target vs. actual containers per order

**Status:** Proposed\
**Persona:** Operator, Key User\
**Goal:** Track and display the number of containers linked to an order against a target.

#### System Behavior

* **Requirement 1:** Maintain counters of currently linked containers: per order, per operator, per product, per referencing resource in the cyber-physical system.

---

### [SPEC-MMM-011] History and audit trail

**Status:** Proposed\
**Persona:** Service Technician, Key User\
**Goal:** Provide a complete, queryable history across all container types and actions.

#### System Behavior

* **Requirement 1:** Persist all link lifecycle events, metadata changes, acknowledgments, and lineage operations with timestamps and operator identifiers.
* **Requirement 2:** Query/filter by container, order, material, time range, station, action type.

---

### [SPEC-MMM-012] Persistence, APIs, and contextual embedding

**Status:** Proposed\
**Persona:** Application Engineer, Platform Developer\
**Goal:** Provide durable storage, interoperable APIs, and system context for the above features.

#### System Behavior

* **Requirement 1:** Data model for: containers (identity via [Identities](../abstractions/index.md#identity), type, properties with controlled vocabularies), links (state, metadata), lineage edges, counters, and audit events.
* **Requirement 2:** Provide a facade for create, read, update, and delete (CRUD) of containers, links (link/unlink), and lineage.
* **Requirement 3:** Provide REST endpoints for create, read, update, and delete (CRUD) of containers, links (link/unlink), and lineage.
* **Requirement 4:** Provide an event stream for create, update and delete events for containers, links (link/unlink), and lineage.
* **Requirement 5:** Integrate with [Orders](../module-orders/index.md) where links reference orders.
* **Requirement 6:** Integrate with [Products](../module-products/index.md) for product/material links.
* **Requirement 7:** Integrate with Operators for operator links.
* **Requirement 8:** Containers are referenceable from the digital twin to embed plant, workplace, and station context; i.e. integrate with [Resources](../module-resources/index.md).
**Failure Handling:** Consistent error codes/messages; transactional integrity for multi-step operations.

---

### [SPEC-MMM-013] Material requests and pending request tracking

**Status:** Proposed\
**Persona:** Operator, Application Engineer\
**Goal:** Enable operators to request material and view pending material requests in the system.

#### System Behavior

* **Requirement 1:** Operator or automation can create a material request: provide product identifier, requested quantity, and optional source location and target order.
* **Requirement 2:** Material requests are persisted with status tracking: pending, partially received, received, cancelled.
* **Requirement 3:** Pending material requests are displayed in the visual interface with: product, quantity, creation time, request status and optional properties.
* **Requirement 4:** Operators can filter and search pending requests by order or product.
* **Requirement 5:** A facade event is published when a material request is created, enabling Application Engineers to wire adapter notifications to external systems.
**Failure Handling:** Invalid order reference, unknown product, or missing quantity prevent request creation with clear user feedback.

#### Constraints

* **Constraint 1:** Material requests are independent tracking entities and do not automatically create container registrations.

---

### [SPEC-MMM-014] Incoming material announcements and operator preparation

**Status:** Proposed\
**Persona:** Operator\
**Goal:** Display announced incoming material in the UI so operators can prepare for arrival and acknowledge notifications.

#### System Behavior

* **Requirement 1:** Application Engineers can submit incoming material announcements to MORYX via facade API: container identity (optional), order reference, product, quantity, and expected arrival time.
* **Requirement 2:** Announcements can be cancelled through the facade API.
* **Requirement 3:** Announcements are persisted with status: announced, partially received, received, cancelled.
* **Requirement 4:** Active announcements are displayed in the visual interface with: product, quantity, expected arrival, status and optionally order.
* **Requirement 5:** When material is registered via [SPEC-MMM-002](#spec-mmm-002-register-material-container-in-the-system) that matches an announced item, the announcement is automatically marked as (partially) received.

---

### [SPEC-MMM-015] Container fill level tracking and consumption updates

**Status:** Proposed\
**Persona:** Operator, Application Engineer\
**Goal:** Track material consumption on containers and update fill levels during production.

#### System Behavior

* **Requirement 1:** When a container is registered, its initial fill level is captured (see [SPEC-MMM-002](#spec-mmm-002-register-material-container-in-the-system)).
* **Requirement 2:** Operators provide consumption updates: container identity, consumed quantity (or remaining quantity).
* **Requirement 3:** MORYX recalculates fill level based on consumption input and updates container metadata.
* **Requirement 4:** Consumption updates trigger hooks that Application Engineers can use to enforce policies (e.g., over-consumption warnings) via [SPEC-MMM-009](#spec-mmm-009-fillinglinking-hooks-and-blocking-policies).
* **Requirement 5:** Current fill level is displayed in the UI and available via REST/facade queries.
**Failure Handling:** Over-consumption (exceeding initial fill level) triggers a configurable hook; operators receive warnings or blocks depending on configuration.

---

### [SPEC-MMM-016] Finished goods pre-advice and material ready notification

**Status:** Proposed\
**Persona:** Operator, Application Engineer\
**Goal:** Enable operators to announce that finished goods or unconsumed material is ready to leave the production station before formal deregistration.

#### System Behavior

* **Requirement 1:** Operator can announce container(s) as ready for departure via the visual interface: provide container identity, product identifier, optionally final fill level, order, and departure reason (e.g., "finished goods", "unused material", "transfer to next station").
* **Requirement 2:** Pre-advice records are persisted with status: announced, completed, cancelled.
* **Requirement 3:** Active pre-advice announcements are displayed in the visual interface with: container identity and announced quantity.
* **Requirement 4:** Application Engineers can query pending pre-advice records via facade API to enable external notifications (e.g., WMS pickup scheduling, next station preparation).
* **Requirement 5:** A facade event is published when a pre-advice is created, enabling Application Engineers to wire adapter notifications to external systems.
* **Requirement 6:** Pre-advice records persist until the container is formally deregistered (see [SPEC-MMM-017](#spec-mmm-017-container-deletion-deregistration-and-material-handover)).
**Failure Handling:** Unknown container identity or already-deregistered container prevents pre-advice creation with clear feedback.

#### Constraints

* **Constraint 1:** Pre-advice is optional; containers can be deregistered directly without prior announcement.

---

### [SPEC-MMM-017] Container deletion (deregistration) and material handover

**Status:** Proposed\
**Persona:** Operator, Application Engineer\
**Goal:** Enable operators to deregister containers when material leaves the production station and finalize container relationships.

#### System Behavior

* **Requirement 1:** Operator can delete (deregister) a container from the production station via the visual interface.
* **Requirement 2:** Deletion unlinks the container from all domain constructs (see [SPEC-MMM-004](#spec-mmm-004-link-containers-to-domain-constructs-and-capture-metadata)).
* **Requirement 3:** Final fill level, consumption quantity, and lineage information (if split/merged during production) are finalized and locked in the audit trail.
* **Requirement 4:** A facade event is published on deletion, enabling Application Engineers to wire downstream notifications (e.g., to WMS, next production station).
* **Requirement 5:** Container metadata and genealogy (see [SPEC-MMM-008](#spec-mmm-008-container-lineage-splitmerge-and-auto-unlink-on-refill)) remain queryable for traceability and reporting after deletion.
**Failure Handling:** Unknown container identity or already-deleted container prevents re-deletion with clear feedback. Incomplete lineage validation (e.g., missing merge source) triggers a warning but allows deletion.

#### Constraints

* **Constraint 1:** Deletion is a one-way operation; re-registration of the same container must be explicit.

## Glossary

### Material Container

A physical container capable of containing or holding a single or multiple types of material used for storage, transport or material provisioning in a cyber-physical system.

### Lineage

The genealogical record of material movement and transformations, tracing splits and merges of containers across operations to enable backward and forward traceability.

### Hook

A configurable extension point in the material management system that fires at defined lifecycle stages (before-link, after-link, before-unlink, after-unlink, before-quantity-change, after-quantity-change). Hooks can allow, block, or require acknowledgment for operations, and can enrich or modify metadata.

### Pre-advice

An announcement that material (finished goods or unconsumed material) is ready to leave the production station before formal deregistration. Enables external systems (e.g., WMS, next station) to prepare for material handover.

### Deregistration

The process of formally removing a material container from the production system, unlinking it from all domain constructs, and finalizing its consumption data and lineage in the audit trail.

### Domain Constructs

System entities to which material containers can be linked: orders, products (or product instances), operators, and resources (workplaces/stations in the plant's digital twin).

### Controlled Vocabulary

A predefined set of allowable values for a property (e.g., hole-size classes 1.2 mm/2.0 mm). Used to enforce type-dependent constraints on container properties and ensure data consistency.
