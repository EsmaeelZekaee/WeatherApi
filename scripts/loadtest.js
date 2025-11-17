import http from 'k6/http';

export let options = {
  vus: 50000,           // تعداد کاربران همزمان
  duration: '30s',      // مدت تست
};

export default function () {
      http.get("http://host.docker.internal:8080/weather"); 
}
