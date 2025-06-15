import axios from 'axios';

const createAxios = axios.create({
    baseURL: 'http://localhost:5241'
}); 

export default createAxios;