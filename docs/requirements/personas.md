# Personas

This document defines personas involved in applying MORYX to real‑world scenarios and operating those applications. It complements the Feature Specification Process described in [index.md](index.md) by helping authors anchor requirements and acceptance criteria in concrete user needs.

How to use this document when writing a spec:
- Identify which personas experience the change and why it matters to them.
- Capture acceptance criteria and examples in the language of affected personas.
- Use the heuristics in [index.md](index.md) to decide whether a change warrants a Feature Specification, then reference the persona impact in the SPEC entry.

---

#### Platform Developer 
They are directly involved in the development of the MORYX Framework. 
They implement feature specifications and fix bugs and issues reported to the project.
They are skilled in software development and can easily retrieve information from the codebase and it's documentation.

#### Application Engineer
They are the executing persona who matches what the MORYX Ecosystem offers to the application requirements of a real world application scenario.
They use existing packages, implement application behaviour in code as intended by the used MORYX components and contribute mainly through bug reports and feature requests, as well as in a consulting fashion.
They are skilled in software development and have deep understanding on application contexts and needs.

#### Service Technician
They are maintaining MORYX applications throughout their lifecycle after commisioning.
They use Framework as well as Application documentation to resolve issues that occur during operation or report them to the responsible parties.
They are skilled engineers capable of processing logs and other application behaviour related information as well as mapping technical documentation to such behavior.

#### Key User
They are operating or consulting on the operation of a MORYX application to generate a benefit. 
They are capable of mapping the domain context to concerns of the application or framework implementation or can at least understand such a mapping.
They are engineers or technically skilled domain experts understanding the main concepts of MORYX and able to act as a translator to others in their domain.

#### Supervisor
They are accountable for the operational performance and compliance of one of multiple MORYX Applicaitons after their commissioning.
They use dashboards and reports to monitor KPIs, manage recipes and schedules at a tactical level, approve or stage changes, and coordinate with Application Engineers and Key Users.
They are operations managers or lead engineers who value predictable behavior, traceability, and safe rollouts over technical detail.

#### Operator
They are operating the MORYX application on a daily basis to generate the intended benefits.
They use HMIs and guided workflows to e.g. execute orders, perform changeovers, acknowledge alarms, and input quality or process data.
They are focused on throughput and safety, need clear instructions, resilient UIs, helpful error messages, and minimal steps to complete tasks.
