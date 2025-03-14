
import React from 'react';
import './App.css'; // Importer CSS-filen
import Register from './components/Register';
import Login from './components/Login';

const App = () => {
    return (
        <div>
            <h1>Liv og Røre</h1>
            <Register />
            <Login />
        </div>
    );
};

export default App;