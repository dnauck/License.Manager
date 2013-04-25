function CustomerListCtrl($scope, $location, Customer) {

    $scope.notificationAlert = { show: false, message: '', type: 'info' };

    $scope.customers = Customer.query(
        function(success, getResponseHeaders) {
            $scope.notificationAlert.show = false;
        },
        function(error, getResponseHeaders) {
            $scope.notificationAlert.show = true;
            $scope.notificationAlert.type = 'error';
            $scope.notificationAlert.message = error.data.responseStatus.message;
        });
    
    $scope.deleteCustomer = function (customer) {
        Customer.delete({ id: customer.id },
            function(success, getResponseHeaders) {
                $scope.customers.splice($scope.customers.indexOf(customer), 1);
                $scope.notificationAlert.show = false;
            },
            function(error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };
}

//CustomerListCtrl.$inject = ['$scope', '$location', 'Customer'];

function CustomerDetailsCtrl($scope, $routeParams, Customer) {

    $scope.notificationAlert = { show: false, message: '', type: 'info' };

    $scope.id = $routeParams.id;
    $scope.customer = Customer.get({ id: $scope.id },
        function(success, getResponseHeaders) {
        },
        function(error, getResponseHeaders) {
            $scope.notificationAlert.show = true;
            $scope.notificationAlert.type = 'error';
            $scope.notificationAlert.message = error.data.responseStatus.message;
        });

    $scope.updateCustomer = function(customer) {
        $scope.customer = Customer.update(customer,
            function(success, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'success';
                $scope.notificationAlert.message = 'Successfuly updated!';
            },
            function(error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };

    $scope.deleteCustomer = function(customer) {
        customer.$delete({},
            function(success, getResponseHeaders) {
            },
            function(error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };
}

//CustomerDetailsCtrl.$inject = ['$scope', '$routeParams', 'Customer'];

function CustomerAddCtrl($scope, $location, Customer) {

    $scope.notificationAlert = { show: false, message: '', type: 'info' };
    $scope.emptyModel = {};

    $scope.addCustomer = function(newCustomer) {
        var cust = new Customer(newCustomer);

        cust.$save({},
            function(success, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'success';
                $scope.notificationAlert.message = 'Successfuly created!';
                
                $location.path('/customers');
            },
            function(error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };

    $scope.cancel = function() {
        $scope.customer = angular.copy($scope.emptyModel);
    };
}

//CustomerAddCtrl.$inject = ['$scope', '$location', 'Customer'];