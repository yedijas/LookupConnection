# Lookup Connection
Simple connection test to multiple URLs. The concept is to look for active connection and connect to one that respond. Since this is checking one-by-one, it cannot exactly be used as a failover method/logic.
This simple round robin logic is useful in case you don't have a load balancer available.
