Feature: GetAllOrders
	A list of completed orders in the MTOGO system

@GetAllOrders
Scenario: Fetch orders from controller endpoint
	Given an authenticated user is logged into the system
	And orders exists in the system
	When the user requests a list of all orders
	Then the system returns a list of all the orders