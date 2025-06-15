import axios from 'axios';

const createAxios = axios.create({
    baseURL: 'http://192.168.1.104:5241'
}); 

export default createAxios;