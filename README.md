
==================================
README
==================================

In order to remove Business Process Flow (BPF) from existing records, it is not enough to just deactivate BPF.
Even when we deactivate BPF, records that are associated with it will still show it with warning message that it is deactivated.

It is true, if you delete BPF from Processes in MS Dynamics CRM, they will be removed (not showed) from the records that were associated with that BPF.
However, what if you do not want to delete default BPFs, like those related to sales process on system entities (Leads, Opportunities and Accounts)?
What if you want to hide these default system BPF from default system entity (i.e. Opportunity)?

This workflow activity code is responsible for removing existing association of the Opportunity entity record with BPF.

---------------------------------------------------------------------------------------------
NOTE: 
Opportunity is picked just as an example, but same principle can be used with other entities.
