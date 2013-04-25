angular.module('CustomerServices', ['ngResource']).
    factory('Customer', function($resource) {
        return $resource('api/customers/:id', { id: '@id'}, {
            'query': { method: 'GET', isArray: true },
            'get': { method: 'GET' },
            'update': { method: 'PUT' },
            'delete': { method: 'DELETE' }
        });
    });