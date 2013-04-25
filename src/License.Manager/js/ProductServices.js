angular.module('ProductServices', ['ngResource']).
    factory('Product', function($resource) {
        return $resource('api/products/:id', { id: '@id' }, {
            'query': { method: 'GET', isArray: true },
            'get': { method: 'GET' },
            'update': { method: 'PUT' },
            'delete': { method: 'DELETE' }
        });
    });