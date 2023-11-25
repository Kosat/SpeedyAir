# Coding Exercise - SpeedyAir.ly Flights Manager

SpeedyAir.ly is a brand-new company that aims to provide efficient and fast air freight services; they currently
have 3 planes the planes are scheduled to fly daily at noon. 

# User Stories - Take-Home

## USER STORY #1
As an inventory management user, I can load a flight schedule similar to the one listed in the Scenario above. For
this story you do not yet need to load the orders. I can also list out the loaded flight schedule on the console.

Expected output:
```
Flight: 1, departure: YUL, arrival: YYZ, day: 1
...
Flight: 6, departure: <departure_city>, arrival: <arrival_city>, day: x
```

## USER STORY #2
As an inventory management user, I can generate flight itineraries by scheduling a batch of orders. These flights
can be used to determine shipping capacity.
- Use the json file attached to load the given orders.
- The orders listed in the json file are listed in priority order ie. 1..N
Expected output:
```
order: order-001, flightNumber: 1, departure: <departure_city>, arrival: <arrival_city>, day: x
...
order: order-099, flightNumber: 1, departure: <departure_city>, arrival: <arrival_city>, day: x
```
if an order has not yet been scheduled, output:
```
order: order-X, flightNumber: not scheduled
```
