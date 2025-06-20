import axios from "axios";

const apiBaseUrl =
  process.env.NODE_ENV === "development"
    ? "http://localhost:5241"
    : process.env.VUE_APP_API_BASE_URL;

const createAxios = axios.create({
  baseURL: apiBaseUrl,
});

export default createAxios;
