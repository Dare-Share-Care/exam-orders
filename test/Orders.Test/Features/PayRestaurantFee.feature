Feature: PayRestaurantFee
	PayRestaurantFee is a service that allows a user to pay a restaurant fee

@PayRestaurantFee
Scenario: Paying a restaurant fee
	Given an authenticated restaurant owner is logged into the system
	When an unpaid restaurant fee exists in the system
	And the restaurant owner provides the necessary payment information
	And the restaurant owner pays the restaurant fee
	Then the restaurant fee is paid
