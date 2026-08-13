import { useState } from 'react';
import {
    Box,
    Typography,
    TextField,
    Button,
    Alert,
    Paper,
    InputAdornment,
    IconButton,
    CircularProgress,
    Avatar
} from '@mui/material';
import {
    LockOutlined,
    EmailOutlined,
    Lock,
    Visibility,
    VisibilityOff
} from '@mui/icons-material';
import { login } from '../services/authService';

function LoginPage({ onLoginSuccess }) {
    // States
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [errorMessage, setErrorMessage] = useState('');
    const [isLoading, setIsLoading] = useState(false);

    // Toggle Password Visibility
    const handleClickShowPassword = () => setShowPassword((show) => !show);

    // Submission
    const handleSubmit = async (event) => {
        event.preventDefault();

        setErrorMessage('');
        setIsLoading(true);

        if (!email || !password) {
            setErrorMessage('Por favor, preencha todos os campos.');
            setIsLoading(false);
            return;
        }

        try {
            await login(email, password);
            if (onLoginSuccess) {
                onLoginSuccess();
            }
        } catch (err) {
            setErrorMessage(err.message || 'Erro ao efetuar login. Tente novamente.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <Box
            sx={{
                minHeight: '100vh',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                background: 'linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%)',
                padding: 2
            }}
        >
            <Paper
                elevation={12}
                sx={{
                    padding: { xs: 3, sm: 5 },
                    width: '100%',
                    maxWidth: 420,
                    borderRadius: 4,
                    backdropFilter: 'blur(10px)',
                    boxShadow: '0 20px 25px -5px rgba(0, 0, 0, 0.3), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center'
                }}
            >
                {/* Ícone de Destaque */}
                <Avatar
                    sx={{
                        m: 1,
                        bgcolor: 'primary.main',
                        width: 56,
                        height: 56,
                        boxShadow: '0 4px 12px rgba(25, 118, 210, 0.4)'
                    }}
                >
                    <LockOutlined sx={{ fontSize: 30 }} />
                </Avatar>

                {/* Título & Subtítulo */}
                <Typography
                    variant="h4"
                    component="h1"
                    align="center"
                    fontWeight="800"
                    sx={{ mt: 1, color: 'text.primary', letterSpacing: '-0.5px' }}
                >
                    HR Directory
                </Typography>
                <Typography
                    variant="body2"
                    color="text.secondary"
                    align="center"
                    sx={{ mb: 3, mt: 0.5 }}
                >
                    Insira as suas credenciais para aceder ao painel
                </Typography>

                {/* Mensagem de Erro */}
                {errorMessage && (
                    <Alert
                        severity="error"
                        sx={{
                            width: '100%',
                            mb: 2,
                            borderRadius: 2
                        }}
                    >
                        {errorMessage}
                    </Alert>
                )}

                {/* Formulário */}
                <Box component="form" onSubmit={handleSubmit} sx={{ width: '100%' }}>
                    <TextField
                        fullWidth
                        label="Email"
                        type="email"
                        margin="normal"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        disabled={isLoading}
                        required
                        InputProps={{
                            startAdornment: (
                                <InputAdornment position="start">
                                    <EmailOutlined color="action" />
                                </InputAdornment>
                            ),
                        }}
                        sx={{
                            '& .MuiOutlinedInput-root': {
                                borderRadius: 2.5,
                            }
                        }}
                    />

                    <TextField
                        fullWidth
                        label="Palavra-passe"
                        type={showPassword ? 'text' : 'password'}
                        margin="normal"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        disabled={isLoading}
                        required
                        InputProps={{
                            startAdornment: (
                                <InputAdornment position="start">
                                    <Lock color="action" />
                                </InputAdornment>
                            ),
                            endAdornment: (
                                <InputAdornment position="end">
                                    <IconButton
                                        aria-label="toggle password visibility"
                                        onClick={handleClickShowPassword}
                                        edge="end"
                                    >
                                        {showPassword ? <VisibilityOff /> : <Visibility />}
                                    </IconButton>
                                </InputAdornment>
                            )
                        }}
                        sx={{
                            '& .MuiOutlinedInput-root': {
                                borderRadius: 2.5,
                            }
                        }}
                    />

                    <Button
                        type="submit"
                        fullWidth
                        variant="contained"
                        size="large"
                        disabled={isLoading}
                        sx={{
                            mt: 3,
                            mb: 1,
                            py: 1.5,
                            borderRadius: 2.5,
                            fontSize: '1rem',
                            fontWeight: 'bold',
                            textTransform: 'none',
                            boxShadow: '0 8px 16px rgba(25, 118, 210, 0.24)',
                            transition: 'all 0.2s ease-in-out',
                            '&:hover': {
                                transform: 'translateY(-2px)',
                                boxShadow: '0 12px 20px rgba(25, 118, 210, 0.32)',
                            }
                        }}
                    >
                        {isLoading ? (
                            <CircularProgress size={26} color="inherit" />
                        ) : (
                            'Entrar'
                        )}
                    </Button>
                </Box>
            </Paper>
        </Box>
    );
}

export default LoginPage;