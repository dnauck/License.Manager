angular.module('AccountServices', ['ngResource']).
    factory('Account', function($resource) {
        return $resource('api/accounts/:id', { id: '@id'}, {
            'query': { method: 'GET', isArray: true },
            'get': { method: 'GET' },
            'update': { method: 'PUT' },
            'delete': { method: 'DELETE' }
        });
    });