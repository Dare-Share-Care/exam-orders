Feature: CreateOrder
	The process of creating an order in the system

@CreateOrder
Scenario: Create a new order
	Given an authenticated customer is logged into the system
	When the user has chosen a menu to order from
	And the user creates a new order
	Then the order is created in the system