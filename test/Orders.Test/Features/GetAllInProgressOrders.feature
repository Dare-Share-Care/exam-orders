Feature: GetAllInProgressOrders
	A list of in progress orders in the MTOGO system

@GetAllInProgressOrders
Scenario: Fetch in progress orders from controller endpoint
	Given a courier user is logged into the system
	And orders exists in the system with the order status ‘in progress’
	When the user requests a list of all orders with the given status
	Then the system returns a list of all the orders with the given status