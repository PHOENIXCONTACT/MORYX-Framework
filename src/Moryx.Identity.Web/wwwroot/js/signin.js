async function signIn(host) {
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    await fetch(host + "/api/auth/signIn", {
        method: 'POST',
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ userName: username, password: password })
    })
        .then(response => {
            if (response.status === 200)
                location.assign("/");
            else {
                const validation = document.getElementById('login-validation');
                validation.classList.remove('hide');
            }
        })
        .catch(err => console.log(err));
}

