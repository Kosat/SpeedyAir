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
![list-schedules-cmd](https://github.com/Kosat/SpeedyAir/assets/153023/962ffa24-310d-4fea-8d21-befc4765188a)

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
![generate-itineraries-cmd](https://github.com/Kosat/SpeedyAir/assets/153023/eb2dfe80-64ac-4c8e-87cb-56e4250b7014)

## Unit tests
![image](https://github.com/Kosat/SpeedyAir/assets/153023/8f1a8b10-ff95-43e2-967f-8471b81e5561)
